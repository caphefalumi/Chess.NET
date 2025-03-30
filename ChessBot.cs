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
        private readonly int QUIESCENCE_DEPTH = 4;  // Depth for quiescence search
        private readonly Random _random = new Random();

        // Time control variables
        private TimeSpan _timeLimit;
        private Stopwatch _timer;
        private bool _shouldStopSearch;

        // Piece values (in centipawns)
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

            // Randomize the move order to add variety and potentially find good moves earlier
            List<Move> shuffledMoves = legalMoves.OrderBy(x => _random.Next()).ToList();

            // Iterative deepening - start with a shallow search and gradually increase depth
            for (int depth = 1; depth <= MAX_DEPTH; depth++)
            {
                if (_shouldStopSearch)
                    break;

                foreach (Move move in shuffledMoves)
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
                    shuffledMoves = OrderMovesByScore(shuffledMoves, currentPlayer);
                }
            }

            _timer.Stop();
            return bestMove ?? shuffledMoves.First(); // Fallback to first move if no best move found
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

            // Base case: if we reached the maximum depth, evaluate the position or enter quiescence search
            if (depth <= 0)
                return Quiescence(alpha, beta, player, 0);

            HashSet<Move> legalMoves = _board.GetAllyMoves(player);

            foreach (Move move in legalMoves)
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
        /// Quiescence search to avoid horizon effect - continues searching in unstable positions
        /// </summary>
        private int Quiescence(int alpha, int beta, Player player, int depth)
        {
            // Check if we're out of time
            if (_timer.Elapsed > _timeLimit)
            {
                _shouldStopSearch = true;
                return 0;
            }

            // Stand-pat score - static evaluation of current position
            int standPat = EvaluatePosition(player);

            if (depth >= QUIESCENCE_DEPTH)
                return standPat;

            if (standPat >= beta)
                return beta;

            if (alpha < standPat)
                alpha = standPat;

            // Only consider captures in quiescence search
            HashSet<Move> captureMoves = GetCaptureMoves(player);

            foreach (Move move in captureMoves)
            {
                if (_shouldStopSearch)
                    break;

                _board.MatchState.MakeMove(move);
                int score = -Quiescence(-beta, -alpha, player.Opponent(), depth + 1);
                _board.MatchState.UnmakeMove();

                if (score >= beta)
                    return beta;

                if (score > alpha)
                    alpha = score;
            }

            return alpha;
        }

        /// <summary>
        /// Get all capture moves for the given player
        /// </summary>
        private HashSet<Move> GetCaptureMoves(Player player)
        {
            HashSet<Move> allMoves = _board.GetAllyMoves(player);
            HashSet<Move> captureMoves = new HashSet<Move>();

            foreach (Move move in allMoves)
            {
                // Check if the move is a capture
                Piece capturedPiece = _board.GetPieceAt(move.To);
                if (capturedPiece != null && capturedPiece.Color != player)
                {
                    captureMoves.Add(move);
                }
            }

            return captureMoves;
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

            // Mobility (count of legal moves)
            score += EvaluateMobility(player);

            // Check and checkmate possibilities
            score += EvaluateKingSafety(player);

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
        /// Evaluate mobility (number of legal moves)
        /// </summary>
        private int EvaluateMobility(Player player)
        {
            int playerMoves = _board.GetAllyMoves(player).Count;
            int opponentMoves = _board.GetAllyMoves(player.Opponent()).Count;
            return (playerMoves - opponentMoves) * 10; // 10 points per extra move
        }

        /// <summary>
        /// Evaluate king safety
        /// </summary>
        private int EvaluateKingSafety(Player player)
        {
            int score = 0;

            // Check status
            if (_board.IsInCheck(player))
                score -= 50; // Being in check is bad

            if (_board.IsInCheck(player.Opponent()))
                score += 50; // Putting opponent in check is good

            return score;
        }

        /// <summary>
        /// Check if the game is in the endgame phase (queens gone or limited material)
        /// </summary>
        private bool IsEndgame()
        {
            // Count queens
            int queenCount = 0;
            int pieceCount = 0;

            foreach (Piece piece in _board.Pieces)
            {
                if (piece.Type == PieceType.Queen)
                    queenCount++;

                if (piece.Type != PieceType.King && piece.Type != PieceType.Pawn)
                    pieceCount++;
            }

            // Endgame if no queens or fewer than 6 pieces excluding kings and pawns
            return queenCount == 0 || pieceCount <= 6;
        }

        /// <summary>
        /// Order moves by their estimated score to improve alpha-beta pruning efficiency
        /// </summary>
        private List<Move> OrderMovesByScore(List<Move> moves, Player player)
        {
            Dictionary<Move, int> moveScores = new Dictionary<Move, int>();

            foreach (Move move in moves)
            {
                // Estimate move value
                int score = EstimateMoveValue(move, player);
                moveScores[move] = score;
            }

            return moves.OrderByDescending(m => moveScores[m]).ToList();
        }

        /// <summary>
        /// Estimate the value of a move without performing deep search
        /// </summary>
        private int EstimateMoveValue(Move move, Player player)
        {
            int score = 0;

            // Capturing moves are valuable
            Piece capturedPiece = _board.GetPieceAt(move.To);
            if (capturedPiece != null)
            {
                // MVV-LVA (Most Valuable Victim - Least Valuable Aggressor)
                Piece movingPiece = _board.GetPieceAt(move.From);
                score += 10 * PieceValues[capturedPiece.Type] - PieceValues[movingPiece.Type];
            }

            // Promotion moves are valuable
            if (move.Type == MoveType.Promotion)
            {
                score += 900; // Value of a queen promotion
            }

            // Center control
            if ((move.To.File >= 2 && move.To.File <= 5) && (move.To.Rank >= 2 && move.To.Rank <= 5))
            {
                score += 10;
            }

            return score;
        }

        /// <summary>
        /// Adjust search depth based on available time
        /// </summary>
        public void AdjustDepthForTime(int remainingTimeMs)
        {
            // Adjust MAX_DEPTH based on available time
            if (remainingTimeMs < 1000) // Less than 1 second
            {
                MAX_DEPTH = 2;
            }
            else if (remainingTimeMs < 5000) // Less than 5 seconds
            {
                MAX_DEPTH = 3;
            }
            else if (remainingTimeMs < 30000) // Less than 30 seconds
            {
                MAX_DEPTH = 4;
            }
            else
            {
                MAX_DEPTH = 5;
            }
        }
    }
}