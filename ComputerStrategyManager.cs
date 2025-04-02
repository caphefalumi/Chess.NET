using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class ComputerStrategyManager
{
    private const string API_URL = "https://chess-api.com/v1"; // New API Endpoint

    private readonly HttpClient _httpClient;

    public ComputerStrategyManager()
    {
        _httpClient = new HttpClient();
    }

    public Dictionary<string, object> GetBestMove(string fen, int depth = 12, int maxThinkingTime = 50)
    {
        try
        {
            Console.WriteLine("Sending API request...");
            Console.WriteLine($"FEN: {fen}");

            object requestData = new
            {
                fen,
                depth,
                maxThinkingTime
            };

            string jsonRequest = JsonConvert.SerializeObject(requestData);
            StringContent content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            HttpResponseMessage response = _httpClient.PostAsync(API_URL, content).Result;
            response.EnsureSuccessStatusCode();

            string responseString = response.Content.ReadAsStringAsync().Result;

            Console.WriteLine("API Response:");
            Console.WriteLine(responseString);

            Dictionary<string, object> jsonResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseString);

            if (jsonResponse != null && jsonResponse.ContainsKey("move"))
            {
                Console.WriteLine($"Best move received: {jsonResponse["move"]}");
                return jsonResponse;
            }
            else
            {
                Console.WriteLine("No valid move received from API.");
                return new Dictionary<string, object> { { "error", "No valid move received from API." } };
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetBestMove: {ex.Message}");
            return new Dictionary<string, object> { { "error", ex.Message } };
        }
    }

    public void ApplyMove(Dictionary<string, object> moveData)
    {
        if (moveData == null || !moveData.ContainsKey("move"))
        {
            Console.WriteLine("No move to apply.");
            return;
        }

        string move = moveData["move"].ToString();
        string flags = moveData.ContainsKey("flags") ? moveData["flags"].ToString() : "";

        Console.WriteLine($"Applying move: {move} with flags: {flags}");

        // Determine the type of move based on flags
        if (flags.Contains("n"))
        {
            Console.WriteLine("Non-capture move");
            // Implement non-capture move logic
        }
        if (flags.Contains("b"))
        {
            Console.WriteLine("Pawn push of two squares");
            // Implement pawn push logic
        }
        if (flags.Contains("e"))
        {
            Console.WriteLine("En passant capture");
            // Implement en passant capture logic
        }
        if (flags.Contains("c"))
        {
            Console.WriteLine("Standard capture");
            // Implement standard capture logic
        }
        if (flags.Contains("p"))
        {
            Console.WriteLine("Promotion");
            // Implement promotion logic
        }
        if (flags.Contains("k"))
        {
            Console.WriteLine("Kingside castling");
            // Implement kingside castling logic
        }
        if (flags.Contains("q"))
        {
            Console.WriteLine("Queenside castling");
            // Implement queenside castling logic
        }

        // TODO: Implement move application logic in the game board
    }
}

