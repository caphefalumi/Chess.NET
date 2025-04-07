using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Chess
{
    public class NetworkManager
    {
        private const int TCP_PORT = 5000;
        private const int UDP_PORT = 5001;

        private TcpListener _server;
        private TcpClient _client;
        private NetworkStream _stream;

        private Thread _receiveThread;
        private bool _isConnected;
        private bool _isServer;

        public event Action<string> OnMoveReceived;
        public event Action<bool> OnConnectionStatusChanged;
        private static NetworkManager _instance;

        public static NetworkManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = new NetworkManager();
            }
            return _instance;
        }

        private NetworkManager()
        {
            _isConnected = false;
            _isServer = false;
        }

        public bool IsConnected 
        { 
            get => _isConnected;
            private set
            {
                if (_isConnected != value)
                {
                    _isConnected = value;
                    OnConnectionStatusChanged?.Invoke(_isConnected);
                }
            }
        }

        public void StartServer()
        {
            _isServer = true;
            Thread udpThread = new Thread(BroadcastServerInfo) { IsBackground = true };
            udpThread.Start();

            Thread serverLoop = new Thread(() =>
            {
                while (_isServer)
                {
                    try
                    {
                        _server = new TcpListener(IPAddress.Any, TCP_PORT);
                        _server.Start();
                        Console.WriteLine("Waiting for client...");

                        _client = _server.AcceptTcpClient();
                        Console.WriteLine("Client connected.");
                        _stream = _client.GetStream();
                        IsConnected = true;

                        _receiveThread = new Thread(ReceiveMove) { IsBackground = true };
                        _receiveThread.Start();

                        while (IsConnected) Thread.Sleep(500);
                        Cleanup();

                        Console.WriteLine("Client disconnected. Restarting server...");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Server error: {ex.Message}");
                        Cleanup();
                    }
                }
            });

            serverLoop.IsBackground = true;
            serverLoop.Start();
        }

        public List<ServerInfo> GetServerInfos()
        {
            return DiscoverAllServerInfos();
        } 

        public void StartClientWithDiscovery(string chosenIP)
        {
            if (string.IsNullOrEmpty(chosenIP))
            {
                Console.WriteLine("No server selected.");
                return;
            }

            try
            {
                _client = new TcpClient(chosenIP, TCP_PORT);
                _stream = _client.GetStream();
                IsConnected = true;

                Console.WriteLine("Connected to server.");
                _receiveThread = new Thread(ReceiveMove) { IsBackground = true };
                _receiveThread.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection failed: {ex.Message}");
            }
        }

        private void BroadcastServerInfo()
        {
            UdpClient udpServer = new UdpClient();
            udpServer.EnableBroadcast = true;
            IPEndPoint broadcast = new IPEndPoint(IPAddress.Broadcast, UDP_PORT);
            string ip = GetLocalIPAddress();
            string name = Environment.UserName;

            var info = new ServerInfo { ip = ip, name = name };
            string json = JsonConvert.SerializeObject(info);
            byte[] data = Encoding.UTF8.GetBytes(json);

            while (_isServer)
            {
                try
                {
                    udpServer.Send(data, data.Length, broadcast);
                    Thread.Sleep(1000);
                }
                catch { }
            }
        }

        public class ServerInfo
        {
            public string ip;
            public string name;
        }

        private List<ServerInfo> DiscoverAllServerInfos(int timeoutSeconds = 5)
        {
            UdpClient udpClient = new UdpClient(UDP_PORT);
            udpClient.Client.ReceiveTimeout = timeoutSeconds * 1000;
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, UDP_PORT);
            HashSet<string> seenIps = new HashSet<string>();
            List<ServerInfo> servers = new List<ServerInfo>();
            DateTime start = DateTime.Now;

            try
            {
                while ((DateTime.Now - start).TotalSeconds < timeoutSeconds)
                {
                    byte[] data = udpClient.Receive(ref remoteEndPoint);
                    string json = Encoding.UTF8.GetString(data);
                    ServerInfo info = JsonConvert.DeserializeObject<ServerInfo>(json);

                    if (!seenIps.Contains(info.ip))
                    {
                        seenIps.Add(info.ip);
                        servers.Add(info);
                        Console.WriteLine($"Discovered: {info.name} ({info.ip})");
                    }
                }
            }
            catch (SocketException) { }

            return servers;
        }

        public void SendMove(string moveNotation)
        {
            if (!IsConnected) 
            {
                Console.WriteLine("[NetworkManager] Not sending move: Not connected");
                return;
            }

            try
            {
                Console.WriteLine($"[NetworkManager] Sending move: {moveNotation}");
                byte[] data = Encoding.UTF8.GetBytes(moveNotation);
                _stream.Write(data, 0, data.Length);
                Console.WriteLine("[NetworkManager] Move sent successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[NetworkManager] Send failed: {ex.Message}");
                Cleanup();
            }
        }

        private void ReceiveMove()
        {
            byte[] buffer = new byte[1024];
            Console.WriteLine("[NetworkManager] Started receiving messages");

            while (IsConnected)
            {
                try
                {
                    Console.WriteLine("[NetworkManager] Waiting for data...");
                    int count = _stream.Read(buffer, 0, buffer.Length);

                    if (count == 0)
                    {
                        Console.WriteLine("[NetworkManager] Connection closed by remote host");
                        throw new Exception("Disconnected");
                    }

                    string moveNotation = Encoding.UTF8.GetString(buffer, 0, count);
                    Console.WriteLine($"[NetworkManager] Received move: {moveNotation}");
                    OnMoveReceived?.Invoke(moveNotation);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[NetworkManager] Receive error: {ex.Message}");
                    IsConnected = false;
                    Cleanup();
                    break;
                }
            }
            Console.WriteLine("[NetworkManager] Stopped receiving messages");
        }

        public void Cleanup()
        {
            IsConnected = false;
            _stream?.Close();
            _client?.Close();
            _server?.Stop();
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
    }
}
