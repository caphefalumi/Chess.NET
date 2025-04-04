using SplashKitSDK;

namespace Chess
{
    public class BoardRenderer
    {
        private readonly int _squareSize;
        private readonly Color _lightColor;
        private readonly Color _darkColor;
        private readonly int _startX;
        private readonly int _startY;
        private Color _textColor = Color.Black; // Color for indices
        private const int Margin = 20; // Space for indices

        private static BoardRenderer _instance;

        public int StartX => _startX;
        public int StartY => _startY;

        private BoardRenderer(int squareSize, int startX, int startY, Color lightColor, Color darkColor)
        {
            _squareSize = squareSize;
            _startX = startX;
            _startY = startY;
            _lightColor = lightColor;
            _darkColor = darkColor;
        }

        public static BoardRenderer GetInstance(int squareSize, int startX, int startY, Color lightColor, Color darkColor)
        {
            if (_instance == null)
            {
                _instance = new BoardRenderer(squareSize, startX, startY, lightColor, darkColor);
            }
            return _instance;
        }

        private void DrawBoard()
        {
            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    bool isLightSquare = ((rank + file) & 1) != 0;
                    Color squareColor = isLightSquare ? _lightColor : _darkColor;

                    int x = _startX + (file * _squareSize);
                    int y = _startY + ((7 - rank) * _squareSize); // Flip rank for correct orientation

                    SplashKit.FillRectangle(squareColor, x, y, _squareSize, _squareSize);
                }
            }
            DrawIndices();
        }

        private void DrawIndices()
        {
            string[] files = { "a", "b", "c", "d", "e", "f", "g", "h" };

            for (int i = 0; i < 8; i++)
            {
                int fileX = _startX + (i * _squareSize) + (_squareSize / 2);
                int rankY = _startY + ((7 - i) * _squareSize) + (_squareSize / 2);

                // Draw file letters (below board)
                SplashKit.DrawText(files[i], _textColor, "Arial", 16, fileX + 25, _startY + (8 * _squareSize) - 20);

                // Draw rank numbers (left side)
                SplashKit.DrawText((i + 1).ToString(), _textColor, "Arial", 16, _startX - Margin + 25, rankY + 15);
            }
        }

        public void Draw() => DrawBoard();

        public void Draw(Rectangle[] backgroundOverlays)
        {
            foreach (Rectangle backgroundOverlay in backgroundOverlays)
            {
                backgroundOverlay?.Draw();
            }
        }

        public void Draw(HashSet<Piece> pieces)
        {
            if (pieces == null)
            {
                Console.WriteLine("ERROR: pieces collection is null");
                return;
            }

            // Create a safe copy of the pieces collection for rendering
            HashSet<Piece> piecesCopy = pieces.ToHashSet();
            foreach (Piece piece in piecesCopy)
            {
                if (piece == null)
                {
                    Console.WriteLine("ERROR: null piece in collection");
                    continue;
                }
                piece.Draw();
            }
        }

        public void Draw(HashSet<Circle> legalMoves)
        {
            foreach (Circle legalMove in legalMoves)
            {
                legalMove.Draw();
            }
        }

        public void Draw(Board board)
        {
            // Draw the board squares
            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    Color squareColor = (rank + file) % 2 == 0 ? _lightColor : _darkColor;
                    Rectangle square = new Rectangle(squareColor, _startX + file * _squareSize, _startY + rank * _squareSize, _squareSize, _squareSize);
                    square.Draw();

                }
            }
            
            // Draw the pieces
            foreach (Piece piece in board.Pieces)
            {
                if (piece.PieceImage != null)
                {
                    float x = _startX + piece.Position.File * _squareSize;
                    float y = _startY + piece.Position.Rank * _squareSize;
                    SplashKit.DrawBitmap(piece.PieceImage, x, y);
                }
            }

            // Draw the highlights
            foreach (Circle highlight in board.BoardHighlights)
            {
                highlight.Draw();
            }

            // Draw the background overlays
            foreach (Rectangle overlay in board.BackgroundOverlays)
            {
                if (overlay != null)
                {
                    overlay.Draw();
                }
            }
        }
    }
}
