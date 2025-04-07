using SplashKitSDK;

namespace Chess
{
    public class MainMenuState : ScreenState
    {
        private readonly Game _game;
        private readonly Board _board;
        private Button _playLocalButton;
        private Button _playOnlineButton;
        private Button _exitButton;
        private Bitmap _logo;

        public MainMenuState(Game game, Board board)
        {
            _game = game;
            _board = board;

            // Initialize UI elements
            _logo = SplashKit.LoadBitmap("ChessLogo", "Resources/chess_logo.png");

            int centerX = SplashKit.ScreenWidth() / 2;
            int currentY = 300;
            
            // Create the three main buttons
            _playLocalButton = new Button("Play Local", centerX - 100, currentY, 200, 50);
            currentY += 70;
            _playOnlineButton = new Button("Play Online", centerX - 100, currentY, 200, 50);
            currentY += 70;
            _exitButton = new Button("Exit", centerX - 100, currentY, 200, 50);
        }

        public override void HandleInput()
        {
            if (_playLocalButton.IsClicked())
            {
                // Go to local game menu (new game, continue, computer, custom)
                _game.ChangeState(new LocalGameMenuScreen(_game, _board));
            }
            else if (_playOnlineButton.IsClicked())
            {
                // Go directly to network selection screen
                _game.ChangeState(new NetworkGameMenuScreen(_game, _board));
            }
            else if (_exitButton.IsClicked())
            {
                SplashKit.CloseAllWindows();
            }
        }

        public override void Update()
        {
            _playLocalButton.Update();
            _playOnlineButton.Update();
            _exitButton.Update();
        }

        public override void Render()
        {
            SplashKit.ClearScreen(Color.White);
            SplashKit.DrawBitmap(_logo, SplashKit.ScreenWidth() / 2 - _logo.Width / 2, 100);

            _playLocalButton.Draw();
            _playOnlineButton.Draw();
            _exitButton.Draw();

            SplashKit.RefreshScreen();
        }

        public override string GetStateName() => "MainMenu";
    }
}
