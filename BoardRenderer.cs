using SplashKitSDK;

namespace Chess
{
    public class BoardRenderer
    {
        private int _squareSize;
        private int _startX;
        private int _startY;
        private Color _lightColor;
        private Color _darkColor;
        private Color _textColor = Color.Black; // Color for indices
        private const int Margin = 20; // Space for indices

        private static BoardRenderer _instance;

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
            var piecesCopy = pieces.ToList();
            foreach (Piece piece in piecesCopy)
            {
                if (piece == null)
                {
                    Console.WriteLine("ERROR: null piece in collection");
                    continue;
                }
                if (piece.Position == null)
                {
                    Console.WriteLine($"ERROR: null position for piece {piece.PieceChar}");
                    continue;
                }
                if (piece.PieceImage == null)
                {
                    Console.WriteLine($"ERROR: null image for piece {piece.PieceChar} at position {piece.Position}");
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
    }
}
