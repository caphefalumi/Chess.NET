using SplashKitSDK;

namespace Chess
{
    public class BoardDrawer
    {
        private int _squareSize;
        private int _startX;
        private int _startY;
        private Color _lightColor;
        private Color _darkColor;
        private HashSet<IShape> _shapes = new HashSet<IShape>();

        private static BoardDrawer? _instance;

        private BoardDrawer(int squareSize, int startX, int startY, Color lightColor, Color darkColor)
        {
            _squareSize = squareSize;
            _startX = startX;
            _startY = startY;
            _lightColor = lightColor;
            _darkColor = darkColor;
        }

        public static BoardDrawer GetInstance(int squareSize, int startX, int startY, Color lightColor, Color darkColor)
        {
            if (_instance == null)
            {
                _instance = new BoardDrawer(squareSize, startX, startY, lightColor, darkColor);
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
                    int y = _startY + (rank * _squareSize);

                    SplashKit.FillRectangle(squareColor, x, y, _squareSize, _squareSize);
                }
            }
        }



        public void Draw()
        {
            DrawBoard();
        }


        public void Draw(HashSet<IShape> shapes)
        {

            foreach (IShape shape in shapes)
            {
                shape.Draw();
            }
        }

        public void Draw(HashSet<Piece> pieces)
        {
            foreach (Piece piece in pieces)
            {
                piece.Draw();
            }
        }

    }
}
