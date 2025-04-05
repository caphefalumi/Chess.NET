using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    /// <summary>
    /// Chess bot implementation using the Stockfish engine
    /// </summary>
    public class StockfishBot : IBot
    {
        private Process _stockfishProcess;
        private StreamWriter _writer;
        private StreamReader _reader;
        private const string STOCKFISH_PATH = "Resources/Scripts/stockfish-windows-x86-64-avx2.exe";

        public StockfishBot()
        {
            InitializeStockfish();
        }

        private void InitializeStockfish()
        {
                Console.WriteLine($"Initializing Stockfish from: {STOCKFISH_PATH}");
                
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = STOCKFISH_PATH,
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };

                _stockfishProcess = new Process { StartInfo = startInfo };
                _stockfishProcess.Start();

                _writer = _stockfishProcess.StandardInput;
                _reader = _stockfishProcess.StandardOutput;
        }

        public async Task<bool> IsAvailableAsync()
        {
            return await Task.FromResult(true);
        }

        public async Task<string> GetBestMoveAsync(string fen, int timeLimit = 1000)
        {
            return await Task.Run(() =>
            {
                Console.WriteLine($"Sending position to Stockfish: {fen}");
                SendCommand($"position fen {fen}");
                SendCommand($"go movetime {timeLimit}");
                
                string response = WaitForResponse("bestmove");
                Console.WriteLine($"Stockfish response: {response}");
                
                // Parse the best move from the response
                // Format: "bestmove e2e4 ponder e7e5"
                string[] lines = response.Split('\n');
                foreach (string line in lines)
                {
                    if (line.Trim().StartsWith("bestmove"))
                    {
                        string[] parts = line.Trim().Split(' ');
                        if (parts.Length >= 2 && parts[0] == "bestmove" && parts[1] != "(none)" && parts[1] != "NULL")
                        {
                            Console.WriteLine($"Found best move: {parts[1]}");
                            return parts[1]; // Return the move in UCI format (e.g., "e2e4")
                        }
                    }
                }
                
                Console.WriteLine("No valid bestmove found in Stockfish response");
                return null;
            });
        }

        private void SendCommand(string command)
        {
            _writer.WriteLine(command);
            _writer.Flush();
        }

        private string WaitForResponse(string expectedResponse)
        {

            StringBuilder response = new StringBuilder();
            string line;
            DateTime startTime = DateTime.Now;
            TimeSpan timeout = TimeSpan.FromSeconds(5); // 5 second timeout
        
            while ((line = _reader.ReadLine()) != null)
            {
                // Check for timeout
                if (DateTime.Now - startTime > timeout)
                {
                    Console.WriteLine("Timeout waiting for Stockfish response");
                    break;
                }
                
                response.AppendLine(line);
                Console.WriteLine($"Stockfish output: {line}");
                
                if (line.Trim().StartsWith(expectedResponse))
                {
                    return response.ToString();
                }
            }
            
            Console.WriteLine($"Did not find expected response '{expectedResponse}' from Stockfish");
            return response.ToString();
        }
    }
} 