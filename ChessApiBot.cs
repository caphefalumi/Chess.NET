using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Chess
{
    public class ChessApiBot : IBot
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private bool _isAvailable = false;
        private const string API_URL = "https://chess-api.com/v1";

        public ChessApiBot()
        {
            try
            {
                _isAvailable = CheckForApiConnectionAsync().Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking API availability: {ex.Message}");
                _isAvailable = false;
            }

            Console.WriteLine(_isAvailable ? "Chess API is available." : "Chess API is not available.");
        }

        private async Task<Dictionary<string, object>> PostApiRequestAsync(Dictionary<string, object> requestBody)
        {
            string jsonRequest = JsonConvert.SerializeObject(requestBody);
            StringContent content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await _httpClient.PostAsync(API_URL, content);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"API error: {response.StatusCode}");
                    _isAvailable = false;
                    return null;
                }

                string responseString = await response.Content.ReadAsStringAsync();
                Console.WriteLine("API Response:");
                Console.WriteLine(responseString);

                return JsonConvert.DeserializeObject<Dictionary<string, object>>(responseString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during API call: {ex.Message}");
                _isAvailable = false;
                return null;
            }
        }

        private async Task<bool> CheckForApiConnectionAsync()
        {
            Dictionary<string, object> response = await PostApiRequestAsync(new Dictionary<string, object>
            {
                { "fen", "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1" }
            });

            return response != null && response.ContainsKey("move");
        }

        public Task<bool> IsAvailableAsync()
        {
            return Task.FromResult(_isAvailable);
        }

        public async Task<string> GetBestMoveAsync(string fen, int timeLimit = 1000)
        {
            Console.WriteLine("Sending API request...");
            Console.WriteLine($"FEN: {fen}");

            if (_isAvailable)
            {
                Dictionary<string, object> response = await PostApiRequestAsync(new Dictionary<string, object>
                {
                    { "fen", fen },
                    { "depth", 12 },
                    { "maxThinkingTime", timeLimit / 10 }
                });

                if (response != null && response.TryGetValue("move", out object move))
                {
                    Console.WriteLine($"Best move received: {move}");
                    return move.ToString();
                }
            }

            return null;
        }
    }
}
