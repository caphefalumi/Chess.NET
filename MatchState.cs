namespace Chess
{
    public class MatchState
    {
        private static MatchState _instance;
        private Board _board;

        private Player _currentPlayer;
        private readonly Stack<KeyValuePair<Move, string>> _moveHistory;

        private bool _canWhiteCastleKingside;
        private bool _canWhiteCastleQueenside;
        private bool _canBlackCastleKingside;
        private bool _canBlackCastleQueenside;

        private int _halfmoveClock;
        private int _fullmoveNumber;

        public Player CurrentPlayer => _currentPlayer;
        public Stack<KeyValuePair<Move, string>> MoveHistory => _moveHistory;
        public bool CanWhiteCastleKingside => _canWhiteCastleKingside;
        public bool CanWhiteCastleQueenside => _canWhiteCastleQueenside;
        public bool CanBlackCastleKingside => _canBlackCastleKingside;
        public bool CanBlackCastleQueenside => _canBlackCastleQueenside;
        public int HalfmoveClock => _halfmoveClock;
        public int FullmoveNumber => _fullmoveNumber;

        private MatchState(Board board, Player startingPlayer)
        {
            _board = board;
            _currentPlayer = startingPlayer;
            _moveHistory = new Stack<KeyValuePair<Move, string>>();

            _canWhiteCastleKingside = true;
            _canWhiteCastleQueenside = true;
            _canBlackCastleKingside = true;
            _canBlackCastleQueenside = true;

            _halfmoveClock = 0;
            _fullmoveNumber = 1;
        }

        public static MatchState GetInstance(Board board, Player startingPlayer)
        {
            if (_instance == null)
            {
                _instance = new MatchState(board, startingPlayer);
            }
            return _instance;
        }


        public void MakeMove(Move move, bool isSimulation = false)
        {
            if (!isSimulation)
            {
                UpdateCastlingRights(move);
                UpdateHalfmoveClock(move);
                if (_currentPlayer == Player.Black)
                {
                    _fullmoveNumber++;
                }
                Console.WriteLine(move.Type);
                move.Sound.Play();
            }

            move.Execute(_board, isSimulation);
            _moveHistory.Push(new KeyValuePair<Move, string>(move, _board.GetFen()));
            _currentPlayer = _currentPlayer.Opponent();
        }

        private void UpdateCastlingRights(Move move)
        {
            if (move.MovedPiece is King)
            {
                if (move.MovedPiece.Color == Player.White)
                {
                    _canWhiteCastleKingside = false;
                    _canWhiteCastleQueenside = false;
                }
                else
                {
                    _canBlackCastleKingside = false;
                    _canBlackCastleQueenside = false;
                }
            }

            if (move.MovedPiece is Rook)
            {
                Position pos = move.From;

                if (pos.Equals(new Position(7, 0)))
                    _canWhiteCastleKingside = false;

                if (pos.Equals(new Position(0, 0)))
                    _canWhiteCastleQueenside = false;

                if (pos.Equals(new Position(7, 7)))
                    _canBlackCastleKingside = false;

                if (pos.Equals(new Position(0, 7)))
                    _canBlackCastleQueenside = false;
            }

            Piece capturedPiece = _board.GetPieceAt(move.To);
            if (capturedPiece is Rook)
            {
                Position pos = move.To;

                if (pos.Equals(new Position(7, 0)))
                    _canWhiteCastleKingside = false;

                if (pos.Equals(new Position(0, 0)))
                    _canWhiteCastleQueenside = false;

                if (pos.Equals(new Position(7, 7)))
                    _canBlackCastleKingside = false;

                if (pos.Equals(new Position(0, 7)))
                    _canBlackCastleQueenside = false;
            }
        }

        private void UpdateHalfmoveClock(Move move)
        {
            if (move.MovedPiece?.Type == PieceType.Pawn || move.CapturedPiece is not null)
            {
                _halfmoveClock = 0;
            }
            else
            {
                _halfmoveClock++;
            }
        }

        public void UnmakeMove(bool isSimulation = false)
        {
            if (_moveHistory.Count > 0)
            {
                Move lastMove = _moveHistory.Pop().Key;
                lastMove.Undo(_board, isSimulation);
                _currentPlayer = _currentPlayer.Opponent();

                if (!isSimulation)
                {
                    if (_currentPlayer == Player.Black)
                    {
                        _fullmoveNumber--;
                    }
                    _halfmoveClock--;
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
            _currentPlayer = Player.White;
            _moveHistory.Clear();

            _canWhiteCastleKingside = true;
            _canWhiteCastleQueenside = true;
            _canBlackCastleKingside = true;
            _canBlackCastleQueenside = true;
            _halfmoveClock = 0;
            _fullmoveNumber = 1;
        }

    }
}
