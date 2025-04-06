using SplashKitSDK;

namespace Chess
{
    public class LocalGameMenuState : ScreenState
    {
        private readonly Game _game;
        private readonly Board _board;
        private Button _newGameButton;
        private Button _continueButton;
        private Button _computerButton;
        private Button _customButton;
        private Button _backButton;
        private Bitmap _logo;

        public LocalGameMenuState(Game game, Board board)
        {
            _game = game;
            _board = board;

            // Initialize UI elements
            _logo = SplashKit.LoadBitmap("ChessLogo", "Resources/chess_logo.png");

            int centerX = SplashKit.ScreenWidth() / 2;
            int currentY = 400;
            
            // Create buttons for local game options
            _newGameButton = new Button("New Game", centerX - 100, currentY, 200, 50);
            currentY += 60;
            
            if (GameSaver.HasAutoSave())
            {
                _continueButton = new Button("Continue", centerX - 100, currentY, 200, 50);
                currentY += 60;
            }
            
            _computerButton = new Button("Play with Computer", centerX - 100, currentY, 200, 50);
            currentY += 60;
            
            _customButton = new Button("Custom Game", centerX - 100, currentY, 200, 50);
            currentY += 60;
            
            _backButton = new Button("Back", centerX - 100, currentY, 200, 50);
        }

        public override void HandleInput()
        {
            if (_newGameButton.IsClicked())
            {
                // Go to time selection for a standard two-player game
                _game.ChangeState(new TimeSelectionScreen(_game, _board, Variant.TwoPlayer));
            }
            else if (_continueButton != null && _continueButton.IsClicked())
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
            else if (_computerButton.IsClicked())
            {
                // Go to time selection for computer game, then to color selection
                _game.ChangeState(new TimeSelectionScreen(_game, _board, Variant.Computer));
            }
            else if (_customButton.IsClicked())
            {
                // Go to custom board setup screen
                _game.ChangeState(new BoardSetupScreen(_game, _board));
            }
            else if (_backButton.IsClicked())
            {
                // Return to main menu
                _game.ChangeState(new MainMenuState(_game, _board));
            }
        }

        public override void Update()
        {
            _newGameButton.Update();
            
            if (GameSaver.HasAutoSave())
            {
                _continueButton.Update();
            }
            
            _computerButton.Update();
            _customButton.Update();
            _backButton.Update();
        }

        public override void Render()
        {
            SplashKit.ClearScreen(Color.White);
            SplashKit.DrawBitmap(_logo, SplashKit.ScreenWidth() / 2 - _logo.Width / 2, 100);

            SplashKit.DrawText("Local Game Options", Color.Black, Font.Get, 24, 
                SplashKit.ScreenWidth() / 2 - 120, 350);

            _newGameButton.Draw();
            
            if (GameSaver.HasAutoSave())
            {
                _continueButton.Draw();
            }
            
            _computerButton.Draw();
            _customButton.Draw();
            _backButton.Draw();

            SplashKit.RefreshScreen();
        }

        public override string GetStateName() => "LocalGameMenu";
    }
}
