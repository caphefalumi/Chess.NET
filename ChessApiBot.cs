using System.Text;
using Newtonsoft.Json;

namespace Chess
{
    /// <summary>
    /// Chess bot implementation using the chess-api.com web service
    /// </summary>
    public class ChessApiBot : IBot
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private const int DEFAULT_INTERNET_CHECK_TIMEOUT = 3000; // milliseconds
        private const string API_URL = "https://chess-api.com/v1";

        public static async Task<bool> CheckForApiConnectionAsync(int timeoutMs = DEFAULT_INTERNET_CHECK_TIMEOUT)
        {
            // Set timeout for the request
            CancellationTokenSource cts = new CancellationTokenSource(timeoutMs);
            
            // Use HttpClient instead of WebRequest
            HttpResponseMessage response = await _httpClient.GetAsync(API_URL, cts.Token);
            return response.IsSuccessStatusCode;

        }
        
        public async Task<bool> IsAvailableAsync()
        {
            return await CheckForApiConnectionAsync();
        }

        public async Task<string> GetBestMoveAsync(string fen, int timeLimit = 1000)
        {
            Console.WriteLine("Sending API request...");
            Console.WriteLine($"FEN: {fen}");
            Dictionary<string, object> request = new Dictionary<string, object>
            {
                { "fen", fen },
                { "depth", 12 },
                { "maxThinkingTime", timeLimit / 10 }
            };

            string jsonRequest = JsonConvert.SerializeObject(request);

            StringContent content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(API_URL, content);
            
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"API error: {response.StatusCode}");
                return null;
            }

            string responseString = await response.Content.ReadAsStringAsync();

            Console.WriteLine("API Response:");
            Console.WriteLine(responseString);

            Dictionary<string, object> jsonResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseString);

            if (jsonResponse != null && jsonResponse.ContainsKey("move"))
            {
                string move = jsonResponse["move"].ToString();
                Console.WriteLine($"Best move received: {move}");
                return move;
            }
            else
            {
                Console.WriteLine("No valid move received from API.");
                return null;
            }
        }
    }
}