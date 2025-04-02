using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Chess
{
    public class ChessBot
    {
        private const int MAX_DEPTH = 3; // Start with depth 3 for good balance of speed/strength
        private const int MATE_SCORE = 10000;
        private const int DRAW_SCORE = 0;

        // Piece values
        private static readonly Dictionary<Type, int> PIECE_VALUES = new()
        {
            { typeof(Pawn), 100 },
            { typeof(Knight), 320 },
            { typeof(Bishop), 330 },
            { typeof(Rook), 500 },
            { typeof(Queen), 900 },
            { typeof(King), 20000 }
        };

        // Piece position tables
        private static readonly int[,] PAWN_POSITION = {
            {  0,  0,  0,  0,  0,  0,  0,  0 },
            { 50, 50, 50, 50, 50, 50, 50, 50 },
            { 10, 10, 20, 30, 30, 20, 10, 10 },
            {  5,  5, 10, 25, 25, 10,  5,  5 },
            {  0,  0,  0, 20, 20,  0,  0,  0 },
            {  5, -5,-10,  10,  10,-10, -5,  5 },
            {  5, 10, 10,-20,-20, 10, 10,  5 },
            {  0,  0,  0,  0,  0,  0,  0,  0 }
        };

        private static readonly int[,] KNIGHT_POSITION = {
            {-50,-40,-30,-30,-30,-30,-40,-50 },
            {-40,-20,  0,  0,  0,  0,-20,-40 },
            {-30,  0, 10, 15, 15, 10,  0,-30 },
            {-30,  5, 15, 20, 20, 15,  5,-30 },
            {-30,  0, 15, 20, 20, 15,  0,-30 },
            {-30,  5, 10, 15, 15, 10,  5,-30 },
            {-40,-20,  0,  5,  5,  0,-20,-40 },
            {-50,-40,-30,-30,-30,-30,-40,-50 }
        };

        private static readonly int[,] BISHOP_POSITION = {
            {-20,-10,-10,-10,-10,-10,-10,-20 },
            {-10,  0,  0,  0,  0,  0,  0,-10 },
            {-10,  0,  5, 10, 10,  5,  0,-10 },
            {-10,  5,  5, 10, 10,  5,  5,-10 },
            {-10,  0, 10, 10, 10, 10,  0,-10 },
            {-10, 10, 10, 10, 10, 10, 10,-10 },
            {-10,  5,  0,  0,  0,  0,  5,-10 },
            {-20,-10,-10,-10,-10,-10,-10,-20 }
        };

        private static readonly int[,] ROOK_POSITION = {
            {  0,  0,  0,  0,  0,  0,  0,  0 },
            {  5, 10, 10, 10, 10, 10, 10,  5 },
            { -5,  0,  0,  0,  0,  0,  0, -5 },
            { -5,  0,  0,  0,  0,  0,  0, -5 },
            { -5,  0,  0,  0,  0,  0,  0, -5 },
            { -5,  0,  0,  0,  0,  0,  0, -5 },
            { -5,  0,  0,  0,  0,  0,  0, -5 },
            {  0,  0,  0,  5,  5,  0,  0,  0 }
        };

        private static readonly int[,] QUEEN_POSITION = {
            {-20,-10,-10, -5, -5,-10,-10,-20 },
            {-10,  0,  0,  0,  0,  0,  0,-10 },
            {-10,  0,  5,  5,  5,  5,  0,-10 },
            { -5,  0,  5,  5,  5,  5,  0, -5 },
            {  0,  0,  5,  5,  5,  5,  0, -5 },
            {-10,  5,  5,  5,  5,  5,  0,-10 },
            {-10,  0,  5,  0,  0,  0,  0,-10 },
            {-20,-10,-10, -5, -5,-10,-10,-20 }
        };

        private static readonly int[,] KING_MIDDLEGAME_POSITION = {
            {-30,-40,-40,-50,-50,-40,-40,-30 },
            {-30,-40,-40,-50,-50,-40,-40,-30 },
            {-30,-40,-40,-50,-50,-40,-40,-30 },
            {-30,-40,-40,-50,-50,-40,-40,-30 },
            {-20,-30,-30,-40,-40,-30,-30,-20 },
            {-10,-20,-20,-20,-20,-20,-20,-10 },
            { 20, 20,  0,  0,  0,  0, 20, 20 },
            { 20, 30, 10,  0,  0, 10, 30, 20 }
        };

        private static readonly int[,] KING_ENDGAME_POSITION = {
            {-50,-40,-30,-20,-20,-30,-40,-50 },
            {-30,-20,-10,  0,  0,-10,-20,-30 },
            {-30,-10, 20, 30, 30, 20,-10,-30 },
            {-30,-10, 30, 40, 40, 30,-10,-30 },
            {-30,-10, 30, 40, 40, 30,-10,-30 },
            {-30,-10, 20, 30, 30, 20,-10,-30 },
            {-30,-30,  0,  0,  0,  0,-30,-30 },
            {-50,-30,-30,-30,-30,-30,-30,-50 }
        };

        private Board _board;
        private readonly Random _random = new Random();

        // Time control variables
        private TimeSpan _timeLimit;
        private Stopwatch _timer;
        private bool _shouldStopSearch;
        
        // Position history tracking to avoid repetitions
        private Dictionary<string, int> _positionHistory = new Dictionary<string, int>();

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
            
            // Update position history for current position
            string currentFen = _board.GetFen();
            if (_positionHistory.ContainsKey(currentFen))
                _positionHistory[currentFen]++;
            else
                _positionHistory[currentFen] = 1;

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
            List<Move> orderedMoves = OrderMovesByScore(legalMoves.ToList(), currentPlayer);

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
                    _board.MatchState.MakeMove(move, true);

                    // Evaluate the position with negamax
                    int score = -Negamax(depth - 1, -beta, -alpha, currentPlayer.Opponent());

                    // Unmake the move
                    _board.MatchState.UnmakeMove(true);

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
                    // Re-order moves for next iteration based on scores
                    orderedMoves = OrderMovesByScore(legalMoves.ToList(), currentPlayer);
                }
            }

            _timer.Stop();
            return bestMove ?? orderedMoves.First(); // Fallback to first move if no best move found
        }

        /// <summary>
        /// Negamax algorithm with alpha-beta pruning
        /// </summary>
        private int Negamax(int depth, int alpha, int beta, Player player)
        {
            // Check if we're out of time
            if (_timer.Elapsed > _timeLimit)
            {
                _shouldStopSearch = true;
                return 0;  // Return neutral value if search is stopped
            }

            // Check for game over conditions
            if (_board.IsInCheck(player) && _board.GetAllyMoves(player).Count == 0)
                return -(MATE_SCORE - depth); // Checkmate (prefer shorter mates)

            if (_board.GetAllyMoves(player).Count == 0)
                return DRAW_SCORE; // Stalemate (draw)

            // Base case: use quiescence search
            if (depth <= 0)
                return Quiescence(alpha, beta, player);

            HashSet<Move> legalMoves = _board.GetAllyMoves(player);
            
            // Sort moves for better alpha-beta pruning
            List<Move> orderedMoves = OrderMovesByScore(legalMoves.ToList(), player);

            foreach (Move move in orderedMoves)
            {
                if (_shouldStopSearch)
                    break;

                _board.MatchState.MakeMove(move, true);
                int score = -Negamax(depth - 1, -beta, -alpha, player.Opponent());
                _board.MatchState.UnmakeMove(true);

                if (score >= beta)
                    return beta; // Beta cutoff

                alpha = Math.Max(alpha, score);
            }

            return alpha;
        }

        /// <summary>
        /// Quiescence search to evaluate all captures beyond the depth limit
        /// </summary>
        private int Quiescence(int alpha, int beta, Player player)
        {
            int standPat = EvaluatePosition(player);
            if (standPat >= beta)
                return beta;
            alpha = Math.Max(alpha, standPat);

            var captures = _board.GetAllyMoves(player)
                .Where(m => m.CapturedPiece != null)
                .OrderByDescending(m => EstimateMoveValue(m))
                .ToList();

            foreach (var move in captures)
            {
                if (_shouldStopSearch)
                    break;

                _board.MatchState.MakeMove(move, true);
                int score = -Quiescence(-beta, -alpha, player.Opponent());
                _board.MatchState.UnmakeMove(true);

                if (score >= beta)
                    return beta;
                if (score > alpha)
                    alpha = score;
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
                int value = PIECE_VALUES[piece.GetType()];
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
                        positionValue = PAWN_POSITION[rank, file];
                        break;
                    case PieceType.Knight:
                        positionValue = KNIGHT_POSITION[rank, file];
                        break;
                    case PieceType.Bishop:
                        positionValue = BISHOP_POSITION[rank, file];
                        break;
                    case PieceType.Rook:
                        positionValue = ROOK_POSITION[rank, file];
                        break;
                    case PieceType.Queen:
                        positionValue = QUEEN_POSITION[rank, file];
                        break;
                    case PieceType.King:
                        positionValue = isEndgame ? KING_ENDGAME_POSITION[rank, file] : KING_MIDDLEGAME_POSITION[rank, file];
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
            int queenCount = _board.Pieces.Count(p => p.Type == PieceType.Queen);
            int rookCount = _board.Pieces.Count(p => p.Type == PieceType.Rook);
            int totalPieces = _board.Pieces.Count;

            // Consider endgame when queens are gone and few pieces remain
            return (queenCount == 0 && totalPieces <= 8) || (totalPieces <= 6);
        }

        /// <summary>
        /// Order moves by their estimated value to improve alpha-beta pruning efficiency
        /// </summary>
        private List<Move> OrderMovesByScore(List<Move> moves, Player player)
        {
            return moves.OrderByDescending(m => EstimateMoveValue(m)).ToList();
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
                int capturedValue = PIECE_VALUES[move.CapturedPiece.GetType()];
                int attackerValue = PIECE_VALUES[move.MovedPiece.GetType()];
                
                // Consider if exchange is favorable (positive) or unfavorable (negative)
                score = 10 * capturedValue - attackerValue;
                
                // Penalize trading higher value pieces for lower value ones more severely
                if (attackerValue > capturedValue)
                {
                    score -= (attackerValue - capturedValue) * 3;
                }
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
            
            // Strong penalty for moves that leave pieces hanging
            bool leavesHangingPiece = IsHangingPiece(move.MovedPiece);
            if (leavesHangingPiece)
            {
                score -= PIECE_VALUES[move.MovedPiece.GetType()] * 5;
            }
            
            // Check if we're making a bad exchange
            if (move.CapturedPiece != null && move.MovedPiece.Type != PieceType.Pawn)
            {
                HashSet<Move> opponentMoves = _board.GetAllyMoves(move.MovedPiece.Color.Opponent());
                bool willBeRecaptured = opponentMoves.Any(m => m.To.Equals(move.To));
                
                if (willBeRecaptured && PIECE_VALUES[move.MovedPiece.GetType()] > PIECE_VALUES[move.CapturedPiece.GetType()])
                {
                    // Heavily penalize sacrificing a higher value piece for a lower value one
                    score -= (PIECE_VALUES[move.MovedPiece.GetType()] - PIECE_VALUES[move.CapturedPiece.GetType()]) * 4;
                }
            }
            
            // Avoid moving to squares attacked by pawns
            var opponentPawns = _board.Pieces
                .Where(p => p.Type == PieceType.Pawn && p.Color == move.MovedPiece.Color.Opponent())
                .ToList();
                
            foreach (var pawn in opponentPawns)
            {
                var attackedSquares = pawn.GetAttackedSquares();
                if (attackedSquares.Any(m => m.To.Equals(move.To)))
                {
                    score -= PIECE_VALUES[move.MovedPiece.GetType()] / 2;
                    break;
                }
            }
            
            _board.MatchState.UnmakeMove(true);
            
            // Check for repetitions
            _board.MatchState.MakeMove(move, true);
            string resultingFen = _board.GetFen();
            _board.MatchState.UnmakeMove(true);
            
            if (_positionHistory.ContainsKey(resultingFen))
            {
                // Penalize moves that lead to repeated positions
                score -= 100 * _positionHistory[resultingFen];
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
                    PIECE_VALUES[m.MovedPiece.GetType()] <= PIECE_VALUES[attackMove.MovedPiece.GetType()]);
                
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
                    score -= PIECE_VALUES[piece.GetType()] / 4;
                    
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
                        score -= PIECE_VALUES[piece.GetType()] / 2;
                    }
                }
            }
            
            return score;
        }
    }
}