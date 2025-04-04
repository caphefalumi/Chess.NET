using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using SplashKitSDK;

namespace Chess
{
    public class NetworkChessManager
    {
        private static NetworkChessManager _instance;
        private NetworkStream _stream;
        private TcpClient _client;
        private TcpListener _server;
        private const int TCP_PORT = 5000;
        private const int UDP_PORT = 5001;
        private bool _isServer;
        private bool _isConnected;
        private Thread _receiveThread;
        private Board _board;
        private Game _game;
        private MatchConfiguration _config;
        private Action<string> _onFenReceived;

        public bool IsConnected => _isConnected;
        public bool IsServer => _isServer;

        private NetworkChessManager() { }

        public static NetworkChessManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = new NetworkChessManager();
            }
            return _instance;
        }

        public void Initialize(Game game, Board board, MatchConfiguration config, Action<string> onFenReceived)
        {
            _game = game;
            _board = board;
            _config = config;
            _onFenReceived = onFenReceived;
        }

        public void StartServer()
        {
            _isServer = true;
            Thread udpThread = new Thread(BroadcastServerIP) { IsBackground = true };
            udpThread.Start();

            Thread serverLoopThread = new Thread(() =>
            {
                while (_isServer)
                {
                    try
                    {
                        _server = new TcpListener(IPAddress.Any, TCP_PORT);
                        _server.Start();
                        SplashKit.WriteLine("Waiting for a player to join...");

                        _client = _server.AcceptTcpClient();
                        SplashKit.WriteLine("Player connected!");

                        _stream = _client.GetStream();
                        _isConnected = true;

                        _receiveThread = new Thread(ReceiveFEN) { IsBackground = true };
                        _receiveThread.Start();

                        SendFEN(_board.GetFen()); // Send initial FEN

                        // Wait here until disconnected
                        while (_isConnected)
                        {
                            Thread.Sleep(500);
                        }

                        SplashKit.WriteLine("Client disconnected. Restarting server...");
                        Cleanup();
                    }
                    catch (Exception ex)
                    {
                        SplashKit.WriteLine($"Server error: {ex.Message}");
                        Cleanup();
                    }
                }
            });

            serverLoopThread.IsBackground = true;
            serverLoopThread.Start();
        }


        public void StartClient(string serverIP)
        {
            try
            {
                _client = new TcpClient(serverIP, TCP_PORT);
                _stream = _client.GetStream();
                _isConnected = true;

                _receiveThread = new Thread(ReceiveFEN) { IsBackground = true };
                _receiveThread.Start();

                SplashKit.WriteLine("Connected to host!");
                _game.ChangeState(new GameplayScreen(_game, _board, _config));
            }
            catch (Exception ex)
            {
                SplashKit.WriteLine($"Connection failed: {ex.Message}");
                Cleanup();
            }
        }

        private void BroadcastServerIP()
        {
            UdpClient udpServer = new UdpClient();
            udpServer.EnableBroadcast = true;
            IPEndPoint broadcastEndPoint = new IPEndPoint(IPAddress.Broadcast, UDP_PORT);

            string localIP = GetLocalIPAddress();
            byte[] data = Encoding.UTF8.GetBytes(localIP);

            while (_isServer)
            {
                try
                {
                    udpServer.Send(data, data.Length, broadcastEndPoint);
                    Thread.Sleep(1000);
                }
                catch { }
            }
        }

        public string DiscoverServerIP()
        {
            UdpClient udpClient = new UdpClient(UDP_PORT);
            udpClient.Client.ReceiveTimeout = 5000;
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, UDP_PORT);

            try
            {
                byte[] receivedData = udpClient.Receive(ref remoteEndPoint);
                return Encoding.UTF8.GetString(receivedData);
            }
            catch (SocketException)
            {
                return null;
            }
        }

        private string GetLocalIPAddress()
        {
            foreach (IPAddress ip in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip.ToString();
            }
            return "127.0.0.1";
        }

        public void SendFEN(string fen)
        {
            if (!_isConnected) return;

            try
            {
                byte[] data = Encoding.UTF8.GetBytes(fen);
                _stream.Write(data, 0, data.Length);
            }
            catch
            {
                SplashKit.WriteLine("Failed to send FEN. Connection lost.");
                Cleanup();
            }
        }

private void ReceiveFEN()
{
    byte[] buffer = new byte[1024];

    while (_isConnected)
    {
        try
        {
            int bytesRead = _stream.Read(buffer, 0, buffer.Length);
            if (bytesRead == 0)
                throw new Exception("Disconnected by peer");

            string receivedFEN = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            _onFenReceived?.Invoke(receivedFEN);
        }
        catch
        {
            SplashKit.WriteLine("Connection lost.");
            _isConnected = false;

            if (_isServer)
            {
                SplashKit.WriteLine("Waiting for new connection...");
            }

            Cleanup();
            break;
        }
    }
}


        public void Cleanup()
        {
            _isConnected = false;
            _stream?.Close();
            _client?.Close();
            _server?.Stop();
        }
    }
} 