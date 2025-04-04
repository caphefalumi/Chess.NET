using SplashKitSDK;

namespace Chess
{
    public class Game
    {
        private static Game _instance;
        private ScreenState _currentState;
        private Board _board;
        private Window _window;
        private MatchState _gameState;

        private Game(string title, int width, int height)
        {
            _window = SplashKit.OpenWindow(title, width, height);

            int squareSize = 80;
            int startX = 0;
            int startY = 0;
            Color lightColor = SplashKit.RGBColor(255, 206, 158);
            Color darkColor = SplashKit.RGBColor(209, 139, 71);

            _board = Board.GetInstance(squareSize, startX, startY, lightColor, darkColor);
            _gameState = MatchState.GetInstance(_board, Player.White);

            _currentState = new MainMenuState(this, _board);
        }

        public static Game GetInstance(string title, int width, int height)
        {
            if (_instance == null)
            {
                _instance = new Game(title, width, height);
            }
            return _instance;
        }

        public static Game GetInstance()
        {
            if (_instance == null)
            {
                _instance = new Game("Chess", 800, 600);
            }
            return _instance;
        }

        public MatchState GetGameState()
        {
            return _gameState;
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
                _currentState.Render();
                _currentState.Update();

            }
        }
    }
}
