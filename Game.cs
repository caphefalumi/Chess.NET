using SplashKitSDK;
using System;

namespace Chess
{
    public class Game
    {
        private ScreenState _currentState;
        private Board _board;
        private Window _window;
        private MatchState _gameState;
        private string _windowName = "Chess";
        private GameEventManager _eventManager;
        private ConsoleDebugObserver _debugObserver;

        public Game(string title, int width, int height)
        {
            _windowName = title;
            _window = SplashKit.OpenWindow(title, width, height);

            int squareSize = 80;
            int startX = 0;
            int startY = 0;
            Color lightColor = SplashKit.RGBColor(255, 206, 158);
            Color darkColor = SplashKit.RGBColor(209, 139, 71);

            // Initialize the observer pattern first to ensure events are captured
            _eventManager = GameEventManager.GetInstance();
            
            // Register the debug observer in debug mode
            #if DEBUG
            _debugObserver = new ConsoleDebugObserver();
            _eventManager.RegisterObserver(_debugObserver);
            #endif

            // Initialize the game components
            _board = Board.GetInstance(squareSize, startX, startY, lightColor, darkColor);
            _gameState = MatchState.GetInstance(_board, Player.White);

            _currentState = new MainMenuState(this, _board);
        }

        public MatchState GetGameState()
        {
            return _gameState;
        }

        public string GetWindowName()
        {
            return _windowName;
        }

        public void ChangeState(ScreenState newState)
        {
            _currentState = newState;
        }

        public void Run()
        {
            // Main game loop
            while (!SplashKit.WindowCloseRequested(_windowName))
            {
                SplashKit.ProcessEvents();

                _currentState.HandleInput();
                _currentState.Update();
                _currentState.Render();
            }

            // Handle auto-save if the current state is GameplayScreen
            if (_currentState is GameplayScreen)
            {
                // Get the current game state and auto-save
                GameplayScreen gameplayScreen = (GameplayScreen)_currentState;
                gameplayScreen.HandleAutoSave();
            }
        }
    }
}
