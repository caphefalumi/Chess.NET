using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Chess
{
    public class ChessBot
    {
        private readonly Board _board;
        private int MAX_DEPTH = 4;  // Default depth limit
        private readonly Random _random = new Random();

        // Time control variables
        private TimeSpan _timeLimit;
        private Stopwatch _timer;
        private bool _shouldStopSearch;

        private static readonly Dictionary<PieceType, int> PieceValues = new Dictionary<PieceType, int>
        {
            { PieceType.Pawn, 100 },
            { PieceType.Knight, 320 },
            { PieceType.Bishop, 330 },
            { PieceType.Rook, 500 },
            { PieceType.Queen, 900 },
            { PieceType.King, 20000 }
        };

        // Piece-Square tables for positional evaluation
        private static readonly int[,] PawnPositionWhite = {
            { 0,  0,  0,  0,  0,  0,  0,  0 },
            { 50, 50, 50, 50, 50, 50, 50, 50 },
            { 10, 10, 20, 30, 30, 20, 10, 10 },
            { 5,  5, 10, 25, 25, 10,  5,  5 },
            { 0,  0,  0, 20, 20,  0,  0,  0 },
            { 5, -5,-10,  0,  0,-10, -5,  5 },
            { 5, 10, 10,-20,-20, 10, 10,  5 },
            { 0,  0,  0,  0,  0,  0,  0,  0 }
        };

        private static readonly int[,] KnightPosition = {
            { -50,-40,-30,-30,-30,-30,-40,-50 },
            { -40,-20,  0,  0,  0,  0,-20,-40 },
            { -30,  0, 10, 15, 15, 10,  0,-30 },
            { -30,  5, 15, 20, 20, 15,  5,-30 },
            { -30,  0, 15, 20, 20, 15,  0,-30 },
            { -30,  5, 10, 15, 15, 10,  5,-30 },
            { -40,-20,  0,  5,  5,  0,-20,-40 },
            { -50,-40,-30,-30,-30,-30,-40,-50 }
        };

        private static readonly int[,] BishopPosition = {
            { -20,-10,-10,-10,-10,-10,-10,-20 },
            { -10,  0,  0,  0,  0,  0,  0,-10 },
            { -10,  0, 10, 10, 10, 10,  0,-10 },
            { -10,  5,  5, 10, 10,  5,  5,-10 },
            { -10,  0,  5, 10, 10,  5,  0,-10 },
            { -10,  5,  5,  5,  5,  5,  5,-10 },
            { -10,  0,  5,  0,  0,  5,  0,-10 },
            { -20,-10,-10,-10,-10,-10,-10,-20 }
        };

        private static readonly int[,] RookPosition = {
            { 0,  0,  0,  0,  0,  0,  0,  0 },
            { 5, 10, 10, 10, 10, 10, 10,  5 },
            {-5,  0,  0,  0,  0,  0,  0, -5 },
            {-5,  0,  0,  0,  0,  0,  0, -5 },
            {-5,  0,  0,  0,  0,  0,  0, -5 },
            {-5,  0,  0,  0,  0,  0,  0, -5 },
            {-5,  0,  0,  0,  0,  0,  0, -5 },
            { 0,  0,  0,  5,  5,  0,  0,  0 }
        };

        private static readonly int[,] QueenPosition = {
            { -20,-10,-10, -5, -5,-10,-10,-20 },
            { -10,  0,  0,  0,  0,  0,  0,-10 },
            { -10,  0,  5,  5,  5,  5,  0,-10 },
            {  -5,  0,  5,  5,  5,  5,  0, -5 },
            {   0,  0,  5,  5,  5,  5,  0, -5 },
            { -10,  5,  5,  5,  5,  5,  0,-10 },
            { -10,  0,  5,  0,  0,  0,  0,-10 },
            { -20,-10,-10, -5, -5,-10,-10,-20 }
        };

        private static readonly int[,] KingMiddleGamePosition = {
            { -30,-40,-40,-50,-50,-40,-40,-30 },
            { -30,-40,-40,-50,-50,-40,-40,-30 },
            { -30,-40,-40,-50,-50,-40,-40,-30 },
            { -30,-40,-40,-50,-50,-40,-40,-30 },
            { -20,-30,-30,-40,-40,-30,-30,-20 },
            { -10,-20,-20,-20,-20,-20,-20,-10 },
            {  20, 20,  0,  0,  0,  0, 20, 20 },
            {  20, 30, 10,  0,  0, 10, 30, 20 }
        };

        private static readonly int[,] KingEndGamePosition = {
            { -50,-40,-30,-20,-20,-30,-40,-50 },
            { -30,-20,-10,  0,  0,-10,-20,-30 },
            { -30,-10, 20, 30, 30, 20,-10,-30 },
            { -30,-10, 30, 40, 40, 30,-10,-30 },
            { -30,-10, 30, 40, 40, 30,-10,-30 },
            { -30,-10, 20, 30, 30, 20,-10,-30 },
            { -30,-30,  0,  0,  0,  0,-30,-30 },
            { -50,-30,-30,-30,-30,-30,-30,-50 }
        };

        public ChessBot(Board board)
        {
            _board = board;
            _timer = new Stopwatch();
        }

        /// <summary>
        /// Gets the best move for the current player using negamax with alpha-beta pruning
        /// </summary>
        /// <param name="timeLimit">Time limit for the search in milliseconds</param>
        /// <returns>The best move found within the time limit</returns>
        public Move GetBestMove(int timeLimit = 1000)
        {
            _shouldStopSearch = false;
            _timeLimit = TimeSpan.FromMilliseconds(timeLimit);
            _timer.Restart();

            Player currentPlayer = _board.MatchState.CurrentPlayer;
            Move bestMove = null;
            int bestScore = int.MinValue;
            int alpha = int.MinValue;
            int beta = int.MaxValue;

            // Get all legal moves for the current player
            HashSet<Move> legalMoves = _board.GetAllyMoves(currentPlayer);
            if (legalMoves.Count == 0)
                return null;

            // If only one move is available, return it immediately
            if (legalMoves.Count == 1)
                return legalMoves.First();

            // Start with a random order of moves
            List<Move> orderedMoves = legalMoves.OrderBy(x => _random.Next()).ToList();

            // Iterative deepening - start with a shallow search and gradually increase depth
            for (int depth = 1; depth <= MAX_DEPTH; depth++)
            {
                if (_shouldStopSearch)
                    break;

                foreach (Move move in orderedMoves)
                {
                    if (_shouldStopSearch)
                        break;

                    // Make the move on the board
                    _board.MatchState.MakeMove(move);

                    // Evaluate the position with negamax
                    int score = -NegaMax(depth - 1, -beta, -alpha, currentPlayer.Opponent());

                    // Unmake the move
                    _board.MatchState.UnmakeMove();

                    // Update best move if better score found
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestMove = move;
                    }

                    // Update alpha
                    alpha = Math.Max(alpha, score);
                }

                // If we've completed a depth without timing out, save the best move at this depth
                if (!_shouldStopSearch)
                {
                    // Order moves for next iteration based on scores
                    orderedMoves = OrderMovesByScore(orderedMoves, currentPlayer);
                }
            }

            _timer.Stop();
            return bestMove ?? orderedMoves.First(); // Fallback to first move if no best move found
        }

        /// <summary>
        /// Negamax algorithm with alpha-beta pruning
        /// </summary>
        private int NegaMax(int depth, int alpha, int beta, Player player)
        {
            // Check if we're out of time
            if (_timer.Elapsed > _timeLimit)
            {
                _shouldStopSearch = true;
                return 0;  // Return neutral value if search is stopped
            }

            // Check for game over conditions
            if (_board.IsInCheck(player) && _board.GetAllyMoves(player).Count == 0)
                return -10000; // Checkmate (-10000 relative to player)

            if (_board.GetAllyMoves(player).Count == 0)
                return 0; // Stalemate (draw)

            // Base case: if we reached the maximum depth, evaluate the position
            if (depth <= 0)
                return EvaluatePosition(player);

            HashSet<Move> legalMoves = _board.GetAllyMoves(player);
            
            // Sort moves for better alpha-beta pruning
            List<Move> orderedMoves = OrderMovesByScore(legalMoves.ToList(), player);

            foreach (Move move in orderedMoves)
            {
                if (_shouldStopSearch)
                    break;

                _board.MatchState.MakeMove(move);
                int score = -NegaMax(depth - 1, -beta, -alpha, player.Opponent());
                _board.MatchState.UnmakeMove();

                if (score >= beta)
                    return beta; // Beta cutoff

                alpha = Math.Max(alpha, score);
            }

            return alpha;
        }

        /// <summary>
        /// Static evaluation of the current position
        /// </summary>
        private int EvaluatePosition(Player player)
        {
            int score = 0;

            // Material difference
            score += EvaluateMaterial(player);

            // Positional factors
            score += EvaluatePositions(player);
            
            // Safety - avoid leaving pieces hanging
            score += EvaluateSafety(player);

            return score;
        }

        /// <summary>
        /// Evaluate material balance for the given player
        /// </summary>
        private int EvaluateMaterial(Player player)
        {
            int score = 0;

            foreach (Piece piece in _board.Pieces)
            {
                int value = PieceValues[piece.Type];
                if (piece.Color == player)
                    score += value;
                else
                    score -= value;
            }

            return score;
        }

        /// <summary>
        /// Evaluate piece positions using piece-square tables
        /// </summary>
        private int EvaluatePositions(Player player)
        {
            int score = 0;
            bool isEndgame = IsEndgame();

            foreach (Piece piece in _board.Pieces)
            {
                int positionValue = 0;
                int rank = piece.Position.Rank;
                int file = piece.Position.File;

                // Flip coordinates for black pieces
                if (piece.Color == Player.Black)
                {
                    rank = 7 - rank;
                    file = 7 - file;
                }

                switch (piece.Type)
                {
                    case PieceType.Pawn:
                        positionValue = PawnPositionWhite[rank, file];
                        break;
                    case PieceType.Knight:
                        positionValue = KnightPosition[rank, file];
                        break;
                    case PieceType.Bishop:
                        positionValue = BishopPosition[rank, file];
                        break;
                    case PieceType.Rook:
                        positionValue = RookPosition[rank, file];
                        break;
                    case PieceType.Queen:
                        positionValue = QueenPosition[rank, file];
                        break;
                    case PieceType.King:
                        positionValue = isEndgame ? KingEndGamePosition[rank, file] : KingMiddleGamePosition[rank, file];
                        break;
                }

                if (piece.Color == player)
                    score += positionValue;
                else
                    score -= positionValue;
            }

            return score;
        }

        /// <summary>
        /// Check if the game is in the endgame phase
        /// </summary>
        private bool IsEndgame()
        {
            int majorPieceCount = 0;
            
            foreach (Piece piece in _board.Pieces)
            {
                if (piece.Type == PieceType.Queen || piece.Type == PieceType.Rook)
                    majorPieceCount++;
            }
            
            return majorPieceCount <= 2;
        }

        /// <summary>
        /// Order moves by their estimated value to improve alpha-beta pruning efficiency
        /// </summary>
        private List<Move> OrderMovesByScore(List<Move> moves, Player player)
        {
            Dictionary<Move, int> moveScores = new Dictionary<Move, int>();

            foreach (Move move in moves)
            {
                // Estimate move value
                int score = EstimateMoveValue(move);
                moveScores[move] = score;
            }

            return moves.OrderByDescending(m => moveScores[m]).ToList();
        }

        /// <summary>
        /// Estimate the value of a move for move ordering
        /// </summary>
        private int EstimateMoveValue(Move move)
        {
            int score = 0;
            
            // Captures are good - prioritize capturing high-value pieces with low-value pieces
            if (move.CapturedPiece != null)
            {
                // MVV-LVA (Most Valuable Victim - Least Valuable Aggressor)
                score = 10 * PieceValues[move.CapturedPiece.Type] - PieceValues[move.MovedPiece.Type];
            }
            
            // Castling is generally good
            if (move.Type == MoveType.CastleKS || move.Type == MoveType.CastleQS)
            {
                score += 300;
            }
            
            // Promotions are good
            if (move.Type == MoveType.Promotion)
            {
                score += 900;  // Assume queen promotion
            }
            
            // Check if the move leaves a piece hanging or undefended
            _board.MatchState.MakeMove(move, true);
            bool leavesHangingPiece = IsHangingPiece(move.MovedPiece);
            _board.MatchState.UnmakeMove(true);
            
            // Heavy penalty for moves that leave pieces hanging
            if (leavesHangingPiece)
            {
                score -= PieceValues[move.MovedPiece.Type] * 2;
            }

            return score;
        }

        /// <summary>
        /// Check if a piece is hanging (can be captured without compensation)
        /// </summary>
        private bool IsHangingPiece(Piece piece)
        {
            if (piece == null) return false;
            
            // Get all opponent moves that can capture this piece
            var opponentMoves = _board.GetAllyMoves(piece.Color.Opponent())
                .Where(m => m.To.Equals(piece.Position))
                .ToList();
            
            if (!opponentMoves.Any()) return false;
            
            // Check if any defending pieces can recapture
            foreach (var attackMove in opponentMoves)
            {
                // Make the capture
                _board.MatchState.MakeMove(attackMove, true);
                
                // See if we can recapture
                var recaptureMoves = _board.GetAllyMoves(piece.Color)
                    .Where(m => m.To.Equals(attackMove.MovedPiece.Position))
                    .ToList();
                
                bool canRecapture = recaptureMoves.Any(m => 
                    PieceValues[m.MovedPiece.Type] <= PieceValues[attackMove.MovedPiece.Type]);
                
                _board.MatchState.UnmakeMove(true);
                
                // If we can't recapture at least one attacker favorably, the piece is hanging
                if (!canRecapture) return true;
            }
            
            return false;
        }

        /// <summary>
        /// Evaluate piece safety to avoid hanging pieces
        /// </summary>
        private int EvaluateSafety(Player player)
        {
            int score = 0;
            var pieces = _board.Pieces.Where(p => p.Color == player).ToList();
            var opponentMoves = _board.GetAllyMoves(player.Opponent());

            foreach (var piece in pieces)
            {
                // Check if this piece is under attack
                bool isUnderAttack = opponentMoves.Any(m => m.To.Equals(piece.Position));
                
                if (isUnderAttack)
                {
                    // Apply penalty based on piece value
                    score -= PieceValues[piece.Type] / 4;
                    
                    // Check if the piece is defended
                    bool isDefended = false;
                    foreach (var allyPiece in pieces.Where(p => p != piece))
                    {
                        if (allyPiece.GetMoves().Any(m => m.To.Equals(piece.Position)))
                        {
                            isDefended = true;
                            break;
                        }
                    }
                    
                    // Extra penalty if not defended
                    if (!isDefended)
                    {
                        score -= PieceValues[piece.Type] / 2;
                    }
                }
            }
            
            return score;
        }

        /// <summary>
        /// Adjust search depth based on available time
        /// </summary>
        public void AdjustDepthForTime(int remainingTimeMs)
        {
            if (remainingTimeMs < 1000)
                MAX_DEPTH = 2;
            else if (remainingTimeMs < 5000)
                MAX_DEPTH = 3;
            else
                MAX_DEPTH = 4;
        }
    }
}
