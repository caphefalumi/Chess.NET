using System.Text;
using Newtonsoft.Json;

namespace Chess
{
    public class ChessApiBot : IBot
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private const int DEFAULT_INTERNET_CHECK_TIMEOUT = 3000;
        private const string API_URL = "https://chess-api.com/v1";

        private async Task<Dictionary<string, object>> PostApiRequestAsync(Dictionary<string, object> requestBody)
        {
            string jsonRequest = JsonConvert.SerializeObject(requestBody);
            StringContent content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            HttpResponseMessage response;

            try
            {
                response = await _httpClient.PostAsync(API_URL, content);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during API call: {ex.Message}");
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"API error: {response.StatusCode}");
                return null;
            }

            string responseString = await response.Content.ReadAsStringAsync();
            Console.WriteLine("API Response:");
            Console.WriteLine(responseString);

            return JsonConvert.DeserializeObject<Dictionary<string, object>>(responseString);
        }

        private async Task<bool> CheckForApiConnectionAsync(int timeoutMs = DEFAULT_INTERNET_CHECK_TIMEOUT)
        {
            Dictionary<string, object> response = await PostApiRequestAsync(new Dictionary<string, object>
            {
                { "fen", "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1" }
            });

            return response.ContainsKey("move");
        }

        public async Task<bool> IsAvailableAsync()
        {
            return await CheckForApiConnectionAsync();
        }

        public async Task<string> GetBestMoveAsync(string fen, int timeLimit = 1000)
        {
            Console.WriteLine("Sending API request...");
            Console.WriteLine($"FEN: {fen}");

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
            return null;
        }
    }
}
