using SplashKitSDK;

namespace Chess
{
    public class MainMenuState : ScreenState
    {
        private readonly Game _game;
        private readonly Board _board;
        private Button _newGameButton;
        private Button _optionsButton;
        private Button _exitButton;
        private Bitmap _logo;

        public MainMenuState(Game game, Board board)
        {
            _game = game;
            _board = board;

            // Initialize UI elements
            _logo = SplashKit.LoadBitmap("ChessLogo", "Resources/chess_logo.png");


            int centerX = SplashKit.ScreenWidth() / 2;
            _newGameButton = new Button("New Game", centerX - 100, 300, 200, 50);
            _optionsButton = new Button("Game Modes", centerX - 100, 370, 200, 50);
            _exitButton = new Button("Exit", centerX - 100, 440, 200, 50);
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
}
