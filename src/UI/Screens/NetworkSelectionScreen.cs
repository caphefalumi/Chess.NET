using SplashKitSDK;

using Chess.UI.States;
using Chess.Core;
using Chess.Pieces;
using Chess.Moves;
using Chess.Interfaces;
using Chess.Networking;
using Chess.Engine;
using Chess.UI.Drawing;
using Chess.Utils;
namespace Chess.UI.Screens
{
    public class NetworkGameMenuScreen : ScreenState
    {
        private readonly Game _game;
        private readonly Board _board;
        private Button _hostButton;
        private Button _joinButton;
        private Button _backButton;
        private TextLabel _statusLabel;
        private TextLabel _screenLabel;
        private List<NetworkManager.ServerInfo> _serverInfos;
        private List<Button> _serverIPButtons;
        private NetworkManager _networkManager;
        private bool _isSearching;
        private MatchConfiguration _config;

        public NetworkGameMenuScreen(Game game, Board board)
        {
            _game = game;
            _board = board;
            _config = new MatchConfiguration { Mode = Variant.Network };
            _networkManager = NetworkManager.GetInstance();
            _serverInfos = new List<NetworkManager.ServerInfo>();
            _serverIPButtons = new List<Button>();
            int centerX = SplashKit.ScreenWidth() / 2;
            _screenLabel = new TextLabel("Network Selection", centerX - 90, 150, Color.Black, 24);
            _hostButton = new Button("Host Game", centerX - 100, 200, 200, 50);
            _joinButton = new Button("Join Game", centerX - 100, 270, 200, 50);
            _backButton = new Button("Back", centerX - 100, 340, 200, 50);
            _statusLabel = new TextLabel("", centerX, 400, Color.Black, 18);
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
                    _statusLabel.Text = "Discorvering...";
                    _statusLabel.Draw();
                    SplashKit.RefreshScreen();
                    _serverInfos = _networkManager.GetServerInfos();
                    _serverIPButtons = CreateButton(_serverInfos);
                    GetChosenIP(_serverIPButtons);
                    _config.NetworkRole = NetworkRole.Client;
                    _statusLabel.Text = "Discorvering...";
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
                _game.ChangeState(new MainMenuScreen(_game, _board));
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
            if (_serverIPButtons.Count > 0)
            {
                foreach (Button button in _serverIPButtons)
                {
                    button.Update();
                }
            }
            if (_isSearching && _serverInfos.Count > 0)
            {
                GetChosenIP(_serverIPButtons);
            }
        }

        public override void Render()
        {
            SplashKit.ClearScreen(Color.White);
            _screenLabel.Draw();
            _hostButton.Draw();
            _joinButton.Draw();
            _backButton.Draw();
            _statusLabel.Draw();
            if (_serverIPButtons.Count > 0)
            {
                _serverIPButtons.ForEach(button => button.Draw());
            }
            SplashKit.RefreshScreen();
        }

        public List<Button> CreateButton(List<NetworkManager.ServerInfo> serverInfos)
        {
            List<Button> serverIPButtons = new List<Button>();
            int index = 0;
            foreach (NetworkManager.ServerInfo info in serverInfos)
            {
                string label = $"{info.name} ({info.ip})";
                Button button = new Button(label, 100, 400 + index * 50, 300, 50, Color.BrightGreen, Color.DarkGreen, Color.Black);
                serverIPButtons.Add(button);
                index++;
            }
            return serverIPButtons;
        }

        public void GetChosenIP(List<Button> serverIPButtons)
        {
            foreach (Button button in serverIPButtons)
            {
                if (button.IsClicked())
                {
                    string ip = button.Text.Split('(')[1].Trim(')', ' ');
                    _networkManager.StartClientWithDiscovery(ip);
                    break;
                }
            }
        }
    }
}
