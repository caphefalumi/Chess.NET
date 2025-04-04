using SplashKitSDK;

namespace Chess
{
    public class NetworkSelectionScreen : ScreenState
    {
        private readonly Game _game;
        private readonly Board _board;
        private readonly Button _hostButton;
        private readonly Button _joinButton;
        private readonly Button _backButton;
        private readonly TextLabel _statusLabel;
        private NetworkChessManager _networkManager;
        private bool _isSearching;
        private string _serverIP;
        private MatchConfiguration _config;

        public NetworkSelectionScreen(Game game, Board board)
        {
            _game = game;
            _board = board;
            _config = new MatchConfiguration { Mode = Variant.Network };
            _networkManager = NetworkChessManager.GetInstance();
            _networkManager.Initialize(_game, _board, _config, OnFenReceived);

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
                    
                    _serverIP = _networkManager.DiscoverServerIP();
                    
                    if (_serverIP != null)
                    {
                        _statusLabel.Text = $"Server found at {_serverIP}. Connecting...";
                        _networkManager.StartClient(_serverIP);
                    }
                    else
                    {
                        _statusLabel.Text = "No server found. Try again.";
                        _isSearching = false;
                        _networkManager.Cleanup();
                    }
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
                _game.ChangeState(new VariantSelectionScreen(_game, _board));
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
            Font titleFont = SplashKit.LoadFont("Arial", "Arial.ttf");
            SplashKit.DrawText("Network Chess", Color.Black, titleFont, 36, 
                SplashKit.ScreenWidth() / 2 - 150, 100);

            _hostButton.Draw();
            _joinButton.Draw();
            _backButton.Draw();
            _statusLabel.Draw();
            SplashKit.RefreshScreen();
        }

        private void OnFenReceived(string fen)
        {
            _board.LoadFen(fen);
        }

        public override string GetStateName() => "NetworkSelection";
    }
} 