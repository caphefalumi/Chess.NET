using SplashKitSDK;

namespace Chess
{
    public class MainMenuState : ScreenState
    {
        private readonly Game _game;
        private readonly Board _board;
        private Button _newGameButton;
        private Button _continueButton;
        private Button _hostGameButton;
        private Button _joinGameButton;
        private Button _optionsButton;
        private Button _exitButton;
        private Bitmap _logo;
        private NetworkManager _networkManager;

        public MainMenuState(Game game, Board board)
        {
            _game = game;
            _board = board;
            _networkManager = new NetworkManager();

            // Initialize UI elements
            _logo = SplashKit.LoadBitmap("ChessLogo", "Resources/chess_logo.png");

            int centerX = SplashKit.ScreenWidth() / 2;
            int currentY = 300;
            _newGameButton = new Button("New Game", centerX - 100, currentY, 200, 50);
            currentY += 70;
            if (GameSaver.HasAutoSave())
            {
                _continueButton = new Button("Continue", centerX - 100, currentY, 200, 50);
                currentY += 70;
            }
            _hostGameButton = new Button("Host Game", centerX - 100, currentY, 200, 50);
            currentY += 70;
            _joinGameButton = new Button("Join Game", centerX - 100, currentY, 200, 50);
            currentY += 70;
            _optionsButton = new Button("Game Modes", centerX - 100, currentY, 200, 50);
            currentY += 70;
            _exitButton = new Button("Exit", centerX - 100, currentY, 200, 50);
        }

        public override void HandleInput()
        {
            if (_newGameButton.IsClicked())
            {
                // Create a MatchConfiguration object with TwoPlayer mode
                MatchConfiguration config = new MatchConfiguration();

                // Pass the configuration to GameplayScreen
                _game.ChangeState(new GameplayScreen(_game, _board, config));
            }
            else if (_continueButton.IsClicked())
            {
                GameSaveData saveData = GameSaver.LoadAutoSave();
                if (saveData != null)
                {
                    // Create configuration with saved variant
                    MatchConfiguration config = new MatchConfiguration 
                    { 
                        Mode = saveData.Variant 
                    };
                    
                    // Create gameplay screen with the config
                    GameplayScreen gameScreen = new GameplayScreen(_game, _board, config);
                    
                    // Load the saved board position
                    _board.LoadFen(saveData.FenString);
                    
                    // Configure clock with saved times
                    Clock clock = Clock.GetInstance(
                        TimeSpan.FromMilliseconds(saveData.WhiteTimeMs),
                        TimeSpan.FromMilliseconds(saveData.IncrementMs)
                    );
                    
                    // Set remaining time for both players
                    clock.SetRemainingTime(Player.White, TimeSpan.FromMilliseconds(saveData.WhiteTimeMs));
                    clock.SetRemainingTime(Player.Black, TimeSpan.FromMilliseconds(saveData.BlackTimeMs));
                    clock.Start();
                    // Change to gameplay screen
                    _game.ChangeState(gameScreen);
                }
            }
            else if (_hostGameButton.IsClicked())
            {
                _networkManager.StartServer();
                MatchConfiguration config = new MatchConfiguration
                {
                    NetworkRole = NetworkRole.Host
                };
                _game.ChangeState(new GameplayScreen(_game, _board, config));
            }
            else if (_joinGameButton.IsClicked())
            {
                _networkManager.StartClientWithDiscovery();
                MatchConfiguration config = new MatchConfiguration
                {
                    NetworkRole = NetworkRole.Client
                };
                _game.ChangeState(new GameplayScreen(_game, _board, config));
            }
            else if (_optionsButton.IsClicked())
            {
                _game.ChangeState(new VariantSelectionScreen(_game, _board));
            }
            else if (_exitButton.IsClicked())
            {
                SplashKit.CloseAllWindows();
            }
        }

        public override void Update()
        {
            _newGameButton.Update();
            if (GameSaver.HasAutoSave())
            {
                _continueButton.Update();
            }
            _hostGameButton.Update();
            _joinGameButton.Update();
            _optionsButton.Update();
            _exitButton.Update();
        }

        public override void Render()
        {
            SplashKit.ClearScreen(Color.White);
            SplashKit.DrawBitmap(_logo, SplashKit.ScreenWidth() / 2 - _logo.Width / 2, 100);

            _newGameButton.Draw();
            if (GameSaver.HasAutoSave())
            {
                _continueButton.Draw();
            }
            _hostGameButton.Draw();
            _joinGameButton.Draw();
            _optionsButton.Draw();
            _exitButton.Draw();

            SplashKit.RefreshScreen();
        }

        public override string GetStateName() => "MainMenu";
    }
}
