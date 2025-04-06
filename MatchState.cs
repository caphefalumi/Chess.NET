using SplashKitSDK;
using System.Collections.Generic;

namespace Chess
{
    public class MatchState
    {
        private static MatchState _instance;
        private Board _board;
        public Player CurrentPlayer { get; private set; }
        public Stack<KeyValuePair<Move, string>> MoveHistory { get; }
        public bool CanWhiteCastleKingside { get; private set; } = true;
        public bool CanWhiteCastleQueenside { get; private set; } = true;
        public bool CanBlackCastleKingside { get; private set; } = true;
        public bool CanBlackCastleQueenside { get; private set; } = true;
        public int HalfmoveClock { get; private set; } = 0;
        public int FullmoveNumber { get; private set; } = 1;

        private MatchState(Board board, Player startingPlayer)
        {
            _board = board;
            CurrentPlayer = startingPlayer;
            MoveHistory = new Stack<KeyValuePair<Move, string>>();
        }

        public static MatchState GetInstance(Board board, Player startingPlayer)
        {
            if (_instance == null)
            {
                _instance = new MatchState(board, startingPlayer);
            }
            return _instance;
        }
        public static MatchState GetInstance()
        {
            return _instance;
        }
        public void MakeMove(Move move, bool isSimulation = false)
        {
            if (!isSimulation)
            {
                UpdateCastlingRights(move);
                UpdateHalfmoveClock(move);
                if (CurrentPlayer == Player.Black)
                {
                    FullmoveNumber++;
                }
                Console.WriteLine(move.Type);
                move.Sound.Play();
            }
            // Execute the move
            move.Execute(_board, isSimulation);
            MoveHistory.Push(new KeyValuePair<Move, string>(move, _board.GetFen()));

            // Switch player
            CurrentPlayer = CurrentPlayer.Opponent();
        }

        private void UpdateCastlingRights(Move move)
        {
            // Check if king is moving (lose all castling rights for that side)
            if (move.MovedPiece is King)
            {
                if (move.MovedPiece.Color == Player.White)
                {
                    CanWhiteCastleKingside = false;
                    CanWhiteCastleQueenside = false;
                }
                else
                {
                    CanBlackCastleKingside = false;
                    CanBlackCastleQueenside = false;
                }
            }

            // Check if rook is moving (lose castling right for that side)
            if (move.MovedPiece is Rook)
            {
                Position pos = move.From;

                // White kingside rook
                if (pos.Equals(new Position(7, 0)))
                    CanWhiteCastleKingside = false;

                // White queenside rook
                if (pos.Equals(new Position(0, 0)))
                    CanWhiteCastleQueenside = false;

                // Black kingside rook
                if (pos.Equals(new Position(7, 7)))
                    CanBlackCastleKingside = false;

                // Black queenside rook
                if (pos.Equals(new Position(0, 7)))
                    CanBlackCastleQueenside = false;
            }

            // Check if rook is captured (lose castling right for that side)
            Piece capturedPiece = _board.GetPieceAt(move.To);
            if (capturedPiece is Rook)
            {
                Position pos = move.To;

                // White kingside rook
                if (pos.Equals(new Position(7, 0)))
                    CanWhiteCastleKingside = false;

                // White queenside rook
                if (pos.Equals(new Position(0, 0)))
                    CanWhiteCastleQueenside = false;

                // Black kingside rook
                if (pos.Equals(new Position(7, 7)))
                    CanBlackCastleKingside = false;

                // Black queenside rook
                if (pos.Equals(new Position(0, 7)))
                    CanBlackCastleQueenside = false;
            }
        }

        private void UpdateHalfmoveClock(Move move)
        {
            // Reset halfmove clock on pawn moves or captures
            if (move.MovedPiece?.Type == PieceType.Pawn || move.CapturedPiece is not null)
            {
                HalfmoveClock = 0;
            }
            else
            {
                // Increment the clock for non-pawn, non-capture moves
                HalfmoveClock++;
            }
        }

        // Update UnmakeMove to restore FEN-related properties
        public void UnmakeMove(bool isSimulation = false)
        {
            if (MoveHistory.Count > 0)
            {
                Move lastMove = MoveHistory.Pop().Key;
                lastMove.Undo(_board, isSimulation);
                CurrentPlayer = CurrentPlayer.Opponent();

                if (!isSimulation)
                {
                    if (CurrentPlayer == Player.Black)
                    {
                        FullmoveNumber--;
                    }
                    HalfmoveClock--;
                }
            }
        }
        public bool MoveResolvesCheck(Move move, Player player)
        {
            MakeMove(move, true);
            bool stillInCheck = _board.IsInCheck(player);
            UnmakeMove(true);
            return !stillInCheck;
        }
        public void Reset()
        {
            _board.ResetBoard();
            CurrentPlayer = Player.White;
            MoveHistory.Clear();
        }

        public void SetCurrentPlayer(Player player)
        {
            CurrentPlayer = player;
        }
    }
}
