using SplashKitSDK;

namespace Chess
{
    public class NetworkSelectionScreen : ScreenState
    {
        private readonly Game _game;
        private readonly Board _board;
        private Button _hostButton;
        private Button _joinButton;
        private Button _backButton;
        private TextLabel _statusLabel;
        private List<string> _serverIPs;
        private List<Button> _serverIPButtons;
        private NetworkManager _networkManager;
        private bool _isSearching;
        private MatchConfiguration _config;

        public NetworkSelectionScreen(Game game, Board board)
        {
            _game = game;
            _board = board;
            _config = new MatchConfiguration { Mode = Variant.Network };
            _networkManager = NetworkManager.GetInstance();
            _serverIPs = new List<string>();
            int centerX = SplashKit.ScreenWidth() / 2;

            _hostButton = new Button("Host Game", centerX - 100, 200, 200, 50);
            _joinButton = new Button("Join Game", centerX - 100, 270, 200, 50);
            _backButton = new Button("Back", centerX - 100, 340, 200, 50);
            _statusLabel = new TextLabel("", centerX - 150, 400, 300, 30);
        }
        


        public override void HandleInput()
        {
            if (_hostButton.IsClicked() && !_isSearching)
            {
                try
                {
                    _isSearching = true;
                    _statusLabel.Text = "Starting server...";
                    
                    _networkManager.StartServer();
                    _config.NetworkRole = NetworkRole.Host;
                    
                    // If we get here, server started successfully
                    _statusLabel.Text = "Server started. Waiting for player...";
                }
                catch (Exception ex)
                {
                    _statusLabel.Text = $"Failed to start server: {ex.Message}";
                    _isSearching = false;
                    _networkManager.Cleanup();
                }
            }
            else if (_joinButton.IsClicked() && !_isSearching)
            {
                try
                {
                    _isSearching = true;
                    _statusLabel.Text = "Searching for server...";
                    _serverIPs = _networkManager.GetServerIPs();
                    GetChosenIP(_serverIPButtons);
                    _config.NetworkRole = NetworkRole.Client;
                    
                    // If we get here, connection attempt started
                    _statusLabel.Text = "Attempting to connect...";
                }
                catch (Exception ex)
                {
                    _statusLabel.Text = $"Connection failed: {ex.Message}";
                    _isSearching = false;
                    _networkManager.Cleanup();
                }
            }
            else if (_backButton.IsClicked())
            {
                _networkManager.Cleanup();
                _game.ChangeState(new MainMenuState(_game, _board));
            }
        }

        public override void Update()
        {
            _hostButton.Update();
            _joinButton.Update();
            _backButton.Update();

            if (_networkManager?.IsConnected == true)
            {
                _isSearching = false;
                _game.ChangeState(new GameplayScreen(_game, _board, _config));
            }
        }

        public override void Render()
        {
            SplashKit.ClearScreen(Color.White);
            
            // Draw title
            SplashKit.DrawText("Network Chess", Color.Black, Font.Arial, 36, SplashKit.ScreenWidth() / 2 - 150, 100);

            _hostButton.Draw();
            _joinButton.Draw();
            _backButton.Draw();
            _statusLabel.Draw();
            _serverIPButtons.ForEach(button => button.Draw());
            SplashKit.RefreshScreen();
        }

        public void GetChosenIP(List<Button> serverIPButtons)
        {
            foreach (Button button in serverIPButtons)
            {
                if (button.IsClicked())
                {
                    _networkManager.StartClientWithDiscovery(button.Text);
                    break;
                }
            }
        }
        public override string GetStateName() => "NetworkSelection";
    }
} 