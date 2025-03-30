using SplashKitSDK;

namespace Chess
{
    public enum Variant
    {
        TwoPlayer,
        Computer,
        SpellChess,
        Custom
    }
    public enum GameResult
    {
        Ongoing,        // Game is still in progress
        Checkmate,      // One player is checkmated
        Stalemate,      // Stalemate (no legal moves, not in check)
        Timeout,        // A player ran out of time
        Resignation,    // A player resigned
        DrawByAgreement, // Players agreed to a draw
        ThreefoldRepetition, // Draw due to threefold repetition
        FiftyMoveRule,  // Draw due to 50-move rule
        InsufficientMaterial // Draw due to insufficient material
    }

    public class Game
    {
        private ScreenState _currentState;
        private Board _board;
        private Window _window;
        private MatchState _GameState;

        public Game(string title, int width, int height)
        {
            _window = SplashKit.OpenWindow(title, width, height);

            int squareSize = 80;
            int startX = 0;
            int startY = 0;
            Color lightColor = SplashKit.RGBColor(255, 206, 158);
            Color darkColor = SplashKit.RGBColor(209, 139, 71);

            _board = Board.GetInstance(squareSize, startX, startY, lightColor, darkColor);
            _GameState = MatchState.GetInstance(_board, Player.White);

            _currentState = new MainMenuState(this, _board);
        }

        public MatchState GetGameState()
        {
            return _GameState;
        }

        public void ChangeState(ScreenState newState)
        {
            _currentState = newState;
        }

        public void Run()
        {
            while (!_window.CloseRequested)
            {
                SplashKit.ProcessEvents();
                _currentState.HandleInput();
                _currentState.Update();
                _currentState.Render();
            }
        }
    }
}
