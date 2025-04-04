using System;
using System.Net.Http;
using System.Text;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;

namespace Chess
{
    /// <summary>
    /// Chess bot implementation using the chess-api.com web service
    /// </summary>
    public class ChessApiBot : IBot
    {
        private readonly HttpClient _httpClient;
        private const string API_URL = "https://chess-api.com/v1";

        public ChessApiBot()
        {
            _httpClient = new HttpClient();
        }

        public async Task<bool> IsAvailableAsync()
        {
            try
            {
                // Check if we can connect to the API by performing a HEAD request
                var request = new HttpRequestMessage(HttpMethod.Head, API_URL);
                var response = await _httpClient.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> GetBestMoveAsync(string fen, int timeLimit = 1000)
        {
            try
            {
                Console.WriteLine("Sending API request...");
                Console.WriteLine($"FEN: {fen}");

                var requestData = new
                {
                    fen = fen,
                    depth = 12,
                    maxThinkingTime = timeLimit/10 // Default time setting from original code
                };

                string jsonRequest = JsonConvert.SerializeObject(requestData);
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
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting best move from API: {ex.Message}");
                return null;
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
} 