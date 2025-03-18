using SplashKitSDK;

namespace Chess
{
    public static class BoardEvent
    {
        private static readonly Dictionary<Position, Move> _moveBuffer = new Dictionary<Position, Move>();
        private static GameState _gameState;
        private static Piece _selectedPiece;
        private static Board _board;

        private static Position GetClickedSquare()
        {
            int file = (int) SplashKit.MouseX() / 80;
            int rank = (int) SplashKit.MouseY() / 80;
            return new Position(file, rank);
        }

        private static void SelectPiece(Position pos)
        {
            _selectedPiece = _board.GetPieceAt(pos);

            if (_selectedPiece != null)
            {
                BufferMoves(_selectedPiece.GetMoves(_board));
                HighlightSelectedPiece();
                HighlightLegalMoves();
            }

        }

        private static void HighlightSelectedPiece()
        {   
            _board.BackgroundOverlays[2] = new Rectangle(SplashKit.RGBAColor(203, 163, 84, 204), _selectedPiece.Position.X, _selectedPiece.Position.Y, _board.SquareSize);
        }
        private static void HighlightLegalMoves()
        {
            _board.BoardHighlights.Clear();
            foreach (Position to in _moveBuffer.Keys)
            {
                int centerX = to.File * 80 + 40;
                int centerY = to.Rank * 80 + 40;
                int radius = _board.IsEmpty(to) ? 9 : 15;
                _board.BoardHighlights.Add(new Circle(Color.SwinburneRed, centerX, centerY, radius));
            }
        }

        private static void HighlightPreviousMove(Position from, Position to)
        {
            Position[] positions = { from, to };

            for (int i = 0; i < positions.Length; i++)
            {
                bool isLightSquare = ((positions[i].Rank + positions[i].File) & 1) != 0;
                Color highlightColor = SplashKit.RGBAColor(203, 163, 84, 204);
                Rectangle rect = new Rectangle(highlightColor, positions[i].X, positions[i].Y, 80, 80);
                _board.BackgroundOverlays[i] = rect;
            }
        }

        private static void MakeMove(Position pos)
        {
            _board.BoardHighlights.Clear();
            _board.BackgroundOverlays[0] = null;
            _board.BackgroundOverlays[1] = null;
            if (_moveBuffer.TryGetValue(pos, out Move move))
            {
                HighlightPreviousMove(move.From, move.To);
                _gameState.MakeMove(move);
                Console.WriteLine(_board.IsInCheck(Player.White));
            }
            else
            {
                SelectPiece(pos);
            }
            _selectedPiece = null;
        }

        private static void BufferMoves(IEnumerable<Move> moves)
        {
            _moveBuffer.Clear();
            foreach (Move move in moves)
            {
                _moveBuffer[move.To] = move;
            }
        }
        public static void HandleMouseEvents(Board board, GameState gameState)
        {
            _board = board;
            _gameState = gameState;
            if (SplashKit.MouseClicked(MouseButton.LeftButton))
            {
                Position pos = GetClickedSquare();

                if (_selectedPiece is null)
                {
                    SelectPiece(pos);
                }
                else
                {
                    MakeMove(pos);
                }
            }
        }
    }
}