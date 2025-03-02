using SplashKitSDK;

namespace Chess
{
    public static class BoardDrawer
    {
        private static int _squareSize;
        private static int _startX;
        private static int _startY;
        private static Color _lightColor;
        private static Color _darkColor;

        public static void Initialize(int squareSize, int startX, int startY, Color lightColor, Color darkColor)
        {
            _squareSize = squareSize;
            _startX = startX;
            _startY = startY;
            _lightColor = lightColor;
            _darkColor = darkColor;
        }

        public static void DrawBoard()
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
    }
}
