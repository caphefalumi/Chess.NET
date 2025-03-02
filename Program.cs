using SplashKitSDK;
namespace Chess
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Create window
            Window window = SplashKit.OpenWindow("Custom Chess Board", 700, 700);

            // Custom colors - change these RGB values to customize
            Color lightColor = SplashKit.RGBColor(255, 206, 158);  // Light square color
            Color darkColor = SplashKit.RGBColor(209, 139, 71);    // Dark square color

            // Chess board parameters
            int squareSize = 80;  // Size of each square in pixels
            int boardSize = squareSize * 8;

            // Center the board in the window
            int startX = (window.Width - boardSize) / 2;
            int startY = (window.Height - boardSize) / 2;

            Board.Create(squareSize, 0, 0, lightColor, darkColor);

            King whiteKing = new King(Color.Violet, "White", new Position(0, 0));

            while (!window.CloseRequested)
            {
                SplashKit.ProcessEvents();
                SplashKit.ClearScreen(Color.White);
                Board.DrawBoard();
                Board.DrawPieces();
                whiteKing.Draw();
                SplashKit.RefreshScreen();
            }

        }
    }
}
