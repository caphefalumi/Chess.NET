using SplashKitSDK;

namespace Chess
{
    // Abstract state class
    public abstract class GameState
    {
        protected Game _game;
        protected Board _board;

        public GameState(Game game, Board board)
        {
            _game = game;
            _board = board;
        }

        public abstract void HandleInput();
        public abstract void Update();
        public abstract void Render();
        public abstract string GetStateName();
    }

    // Main menu state
    public class MainMenuState : GameState
    {
        private Button _newGameButton;
        private Button _optionsButton;
        private Button _exitButton;
        private Bitmap _logo;

        public MainMenuState(Game game, Board board) : base(game, board)
        {
            // Initialize UI elements
            _logo = new Bitmap("ChessLogo", "images/chess_logo.png");
            int centerX = SplashKit.ScreenWidth() / 2;

            _newGameButton = new Button("New Game", centerX - 100, 300, 200, 50);
            _optionsButton = new Button("Game Modes", centerX - 100, 370, 200, 50);
            _exitButton = new Button("Exit", centerX - 100, 440, 200, 50);
        }

        public override void HandleInput()
        {
            if (_newGameButton.IsClicked())
            {
                _game.ChangeState(new GamePlayState(_game, _board, GameMode.Standard));
            }
            else if (_optionsButton.IsClicked())
            {
                _game.ChangeState(new GameModeSelectionState(_game, _board));
            }
            else if (_exitButton.IsClicked())
            {
                SplashKit.CloseAllWindows();
            }
        }

        public override void Update()
        {
            _newGameButton.Update();
            _optionsButton.Update();
            _exitButton.Update();
        }

        public override void Render()
        {
            SplashKit.ClearScreen(Color.White);
            SplashKit.DrawBitmap(_logo, SplashKit.ScreenWidth() / 2 - _logo.Width / 2, 100);

            _newGameButton.Draw();
            _optionsButton.Draw();
            _exitButton.Draw();

            SplashKit.RefreshScreen();
        }

        public override string GetStateName() => "MainMenu";
    }

    // Game mode selection state
    public class GameModeSelectionState : GameState
    {
        private Button _standardModeButton;
        private Button _timedModeButton;
        private Button _customModeButton;
        private Button _backButton;

        public GameModeSelectionState(Game game, Board board) : base(game, board)
        {
            int centerX = SplashKit.ScreenWidth() / 2;

            _standardModeButton = new Button("Standard Mode", centerX - 100, 200, 200, 50);
            _timedModeButton = new Button("Timed Mode", centerX - 100, 270, 200, 50);
            _customModeButton = new Button("Custom Setup", centerX - 100, 340, 200, 50);
            _backButton = new Button("Back", centerX - 100, 410, 200, 50);
        }

        public override void HandleInput()
        {
            if (_standardModeButton.IsClicked())
            {
                _game.ChangeState(new GamePlayState(_game, _board, GameMode.Standard));
            }
            else if (_timedModeButton.IsClicked())
            {
                _game.ChangeState(new GamePlayState(_game, _board, GameMode.Timed));
            }
            else if (_customModeButton.IsClicked())
            {
                _game.ChangeState(new GameSetupState(_game, _board));
            }
            else if (_backButton.IsClicked())
            {
                _game.ChangeState(new MainMenuState(_game, _board));
            }
        }

        public override void Update()
        {
            _standardModeButton.Update();
            _timedModeButton.Update();
            _customModeButton.Update();
            _backButton.Update();
        }

        public override void Render()
        {
            SplashKit.ClearScreen(Color.White);

            SplashKit.DrawText("Select Game Mode", Color.Black, "Arial", 24,
                SplashKit.ScreenWidth() / 2 - 100, 100);

            _standardModeButton.Draw();
            _timedModeButton.Draw();
            _customModeButton.Draw();
            _backButton.Draw();

            SplashKit.RefreshScreen();
        }

        public override string GetStateName() => "GameModeSelection";
    }

    // Gameplay state
    public class GamePlayState : GameState
    {
        private readonly GameMode _gameMode;
        private Button _menuButton;
        private Button _undoButton;
        private Button _resetButton;
        private Chess.GameState _chessGameState;

        public GamePlayState(Game game, Board board, GameMode gameMode) : base(game, board)
        {
            _gameMode = gameMode;
            _chessGameState = Chess.GameState.GetInstance(board, Player.White);

            _menuButton = new Button("Menu", 610, 10, 80, 30);
            _undoButton = new Button("Undo", 610, 50, 80, 30);
            _resetButton = new Button("Reset", 610, 90, 80, 30);
        }

        public override void HandleInput()
        {
            // Handle chess game events
            BoardEvent.HandleMouseEvents(_board, _chessGameState);

            if (_menuButton.IsClicked())
            {
                _game.ChangeState(new MainMenuState(_game, _board));
            }
            else if (_undoButton.IsClicked())
            {
                _chessGameState.UnmakeMove();
            }
            else if (_resetButton.IsClicked())
            {
                _board.Pieces = PieceFactory.CreatePieces(_board);
                _board.BoardHighlights.Clear();
                _chessGameState = Chess.GameState.GetInstance(_board, Player.White);
            }

            // Handle key events for reset and undo as before
            if (SplashKit.KeyTyped(KeyCode.RKey))
            {
                _board.Pieces = PieceFactory.CreatePieces(_board);
                _board.BoardHighlights.Clear();
            }
            else if (SplashKit.KeyTyped(KeyCode.ZKey))
            {
                _chessGameState.UnmakeMove();
            }
        }

        public override void Update()
        {
            _menuButton.Update();
            _undoButton.Update();
            _resetButton.Update();

            // Additional logic for timed mode
            if (_gameMode == GameMode.Timed)
            {
                // Update timer logic here
            }
        }

        public override void Render()
        {
            SplashKit.ClearScreen(Color.White);

            // Draw the board and pieces
            _board.Draw();

            // Draw UI elements
            _menuButton.Draw();
            _undoButton.Draw();
            _resetButton.Draw();

            // Draw game mode specific UI elements
            if (_gameMode == GameMode.Timed)
            {
                // Draw timer UI
                SplashKit.DrawText("Time: XX:XX", Color.Black, "Arial", 16, 610, 130);
            }

            // Draw current player indicator
            string currentPlayer = _chessGameState.CurrentPlayer == Player.White ? "White" : "Black";
            SplashKit.DrawText($"Current Player: {currentPlayer}", Color.Black, "Arial", 16, 10, 650);

            SplashKit.RefreshScreen();
        }

        public override string GetStateName() => "GamePlay";
    }

    // Game setup state for custom board configurations
    public class GameSetupState : GameState
    {
        private Button _startButton;
        private Button _backButton;

        public GameSetupState(Game game, Board board) : base(game, board)
        {
            int centerX = SplashKit.ScreenWidth() / 2;

            _startButton = new Button("Start Game", centerX - 100, 600, 200, 50);
            _backButton = new Button("Back", 10, 10, 80, 30);
        }

        public override void HandleInput()
        {
            // Handle piece placement logic for custom setup

            if (_startButton.IsClicked())
            {
                _game.ChangeState(new GamePlayState(_game, _board, GameMode.Custom));
            }
            else if (_backButton.IsClicked())
            {
                _game.ChangeState(new GameModeSelectionState(_game, _board));
            }
        }

        public override void Update()
        {
            _startButton.Update();
            _backButton.Update();
        }

        public override void Render()
        {
            SplashKit.ClearScreen(Color.White);

            SplashKit.DrawText("Custom Game Setup", Color.Black, "Arial", 24,
                SplashKit.ScreenWidth() / 2 - 120, 20);

            _board.Draw();
            _startButton.Draw();
            _backButton.Draw();

            SplashKit.RefreshScreen();
        }

        public override string GetStateName() => "GameSetup";
    }
}