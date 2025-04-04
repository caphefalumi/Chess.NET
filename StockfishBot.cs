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
        private string STOCKFISH_PATH = "Resources/Scripts/stockfish-windows-x86-64-avx2.exe";
        private bool _isInitialized;

        public StockfishBot()
        {
            InitializeStockfish();
        }

        private void InitializeStockfish()
        {
            try
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

                // Initialize Stockfish
                Console.WriteLine("Sending uci command to Stockfish");
                SendCommand("uci");
                string response = WaitForResponse("uciok");
                Console.WriteLine("Stockfish initialization response:");
                Console.WriteLine(response);
                
                _isInitialized = true;
                Console.WriteLine("Stockfish initialized successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing Stockfish: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                _isInitialized = false;
            }
        }

        public async Task<bool> IsAvailableAsync()
        {
            return await Task.FromResult(_isInitialized);
        }

        public async Task<string> GetBestMoveAsync(string fen, int timeLimit = 1000)
        {
            if (!_isInitialized)
            {
                Console.WriteLine("Stockfish is not initialized. Cannot get best move.");
                return null;
            }

            return await Task.Run(() =>
            {
                try
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
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error getting best move from Stockfish: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");
                    return null;
                }
            });
        }

        private void SendCommand(string command)
        {
            if (!_isInitialized || _writer == null)
            {
                Console.WriteLine($"Cannot send command '{command}': Stockfish not initialized or writer is null");
                return;
            }

            try
            {
                _writer.WriteLine(command);
                _writer.Flush();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending command '{command}' to Stockfish: {ex.Message}");
                _isInitialized = false;
            }
        }

        private string WaitForResponse(string expectedResponse)
        {
            if (!_isInitialized || _reader == null)
            {
                Console.WriteLine($"Cannot wait for response '{expectedResponse}': Stockfish not initialized or reader is null");
                return string.Empty;
            }

            try
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
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading response from Stockfish: {ex.Message}");
                _isInitialized = false;
                return string.Empty;
            }
        }
    }
} 