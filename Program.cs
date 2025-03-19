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
            Color lightColor = SplashKit.RGBColor(255, 206, 158);
            Color darkColor = SplashKit.RGBColor(209, 139, 71);

            // Chess board parameters
            int squareSize = 80;  // Size of each square in pixels
            int boardSize = squareSize * 8;

            // Center the board in the window
            int startX = 0;
            int startY = 0;

            Board board = Board.GetInstance(squareSize, startX, startY, lightColor, darkColor);
            GameState gameState = new GameState(board, Player.White);

            while (!window.CloseRequested)
            {
                SplashKit.ProcessEvents();
                SplashKit.ClearScreen(Color.White);

                // Handle selection and movement of pieces
                BoardEvent.HandleMouseEvents(board, gameState);
                if (SplashKit.KeyTyped(KeyCode.RKey))
                {
                    board.Pieces = PieceFactory.CreatePieces();
                }
                else if (SplashKit.KeyTyped(KeyCode.ZKey))
                {
                    gameState.UnmakeMove();
                }
                 // Draw the board and pieces
                 board.Draw();

                SplashKit.RefreshScreen();
            }
        }
    }
}