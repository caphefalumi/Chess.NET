using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Chess
{
    /// <summary>
    /// Main chess bot class that manages different engine implementations
    /// Uses online API when available, falls back to local Stockfish if offline
    /// </summary>
    public class ChessBot
    {
        private readonly Board _board;
        private readonly IBot _apiBot;
        private readonly IBot _stockfishBot;
        private readonly IMoveConverter _moveConverter;
        private static readonly HttpClient _httpClient = new HttpClient();
        private const int DEFAULT_INTERNET_CHECK_TIMEOUT = 3000; // milliseconds
        private const int DEFAULT_ENGINE_TIMEOUT = 1000; // milliseconds

        public ChessBot(Board board)
        {
            _board = board ?? throw new ArgumentNullException(nameof(board));
            _apiBot = new ChessApiBot();
            _stockfishBot = new StockfishBot();
            _moveConverter = new UciMoveConverter(board);
        }


        public static async Task<bool> CheckForInternetConnectionAsync(int timeoutMs = 10000)
        {
            try
            {
                string url = "https://www.google.com/";

                // Set timeout for the request
                var cts = new CancellationTokenSource(timeoutMs);
                
                // Use HttpClient instead of WebRequest
                var response = await _httpClient.GetAsync(url, cts.Token);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<Move> GetBestMove(int timeLimit = DEFAULT_ENGINE_TIMEOUT)
        {
            // Get current position in FEN format
            string fen = _board.GetFen();
            string bestMoveUci = null;
            
            LogInfo($"FEN: {fen}");
            
            try
            {
                bestMoveUci = await TryGetBestMoveFromAvailableEngines(fen, timeLimit);
            }
            catch (Exception ex)
            {
                LogError($"Error getting best move: {ex.Message}");
            }
            
            if (string.IsNullOrEmpty(bestMoveUci))
            {
                return GetRandomLegalMove();
            }

            // Convert UCI move to our Move object
            Move move = _moveConverter.ConvertUciToMove(bestMoveUci);
            if (move == null)
            {
                LogError($"Failed to convert UCI move: {bestMoveUci}");
                return GetRandomLegalMove();
            }
            
            return move;
        }

        private async Task<string> TryGetBestMoveFromAvailableEngines(string fen, int timeLimit)
        {
            // Check for internet connection first
            bool hasInternetConnection = await CheckForInternetConnectionAsync(DEFAULT_INTERNET_CHECK_TIMEOUT);
            
            // Try to use the API only if we have internet
            if (hasInternetConnection)
            {
                LogInfo("Internet connection available, trying to use Chess API...");
                string apiMove = await _apiBot.GetBestMoveAsync(fen, timeLimit);
                
                if (!string.IsNullOrEmpty(apiMove))
                {
                    LogInfo($"Using API move: {apiMove}");
                    return apiMove;
                }
                
                LogInfo("API returned no move, falling back to Stockfish");
            }
            
            // If API failed or no internet, try Stockfish
            return await TryGetStockfishMove(fen, timeLimit);
        }

        private async Task<string> TryGetStockfishMove(string fen, int timeLimit)
        {
            LogInfo("Trying to use Stockfish...");
            if (await _stockfishBot.IsAvailableAsync())
            {
                string stockfishMove = await _stockfishBot.GetBestMoveAsync(fen, timeLimit);
                if (!string.IsNullOrEmpty(stockfishMove))
                {
                    LogInfo($"Using Stockfish move: {stockfishMove}");
                    return stockfishMove;
                }
                
                LogInfo("Stockfish returned no move");
            }
            else
            {
                LogInfo("Stockfish not available");
            }
            
            return null;
        }

        private Move GetRandomLegalMove()
        {
            LogInfo("Both engines failed, using random move");
            HashSet<Move> legalMoves = _board.GetAllyMoves(_board.MatchState.CurrentPlayer);
            return legalMoves.Count > 0 ? legalMoves.First() : null;
        }

        // Helper methods for logging
        private void LogInfo(string message)
        {
            Console.WriteLine(message);
        }

        private void LogError(string message)
        {
            Console.WriteLine(message);
        }
    }

    /// <summary>
    /// Interface for converting UCI moves to game-specific Move objects
    /// </summary>
    public interface IMoveConverter
    {
        Move ConvertUciToMove(string uciMove);
    }

    /// <summary>
    /// Converts UCI format moves to game-specific Move objects
    /// </summary>
    public class UciMoveConverter : IMoveConverter
    {
        private readonly Board _board;

        public UciMoveConverter(Board board)
        {
            _board = board ?? throw new ArgumentNullException(nameof(board));
        }

        public Move ConvertUciToMove(string uciMove)
        {
            if (uciMove?.Length < 4) return null;

            try
            {
                // Parse source and destination positions
                string sourceStr = uciMove.Substring(0, 2);
                string destStr = uciMove.Substring(2, 2);
                
                Position source = new Position(sourceStr);
                Position destination = new Position(destStr);

                // Get the piece at the source position
                Piece piece = _board.GetPieceAt(source);
                if (piece == null) return null;

                // Check for promotion
                if (uciMove.Length == 5)
                {
                    char promotionPiece = uciMove[4];
                    return new PromotionMove(source, destination, piece, GetPromotionPieceType(promotionPiece));
                }

                // Check for castling
                if (piece is King king && Math.Abs(source.File - destination.File) > 1)
                {
                    MoveType castleType = destination.File > source.File ? MoveType.CastleKS : MoveType.CastleQS;
                    return new CastleMove(castleType, source, king);
                }

                // Check for en passant
                if (piece is Pawn pawn && destination.File != source.File && _board.GetPieceAt(destination) == null)
                {
                    return new EnPassantMove(source, destination, pawn);
                }

                // Default to normal move
                return new NormalMove(source, destination, piece);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error converting move: {ex.Message}");
                return null;
            }
        }

        private PieceType GetPromotionPieceType(char promotionChar)
        {
            return promotionChar switch
            {
                'q' => PieceType.Queen,
                'r' => PieceType.Rook,
                'b' => PieceType.Bishop,
                'n' => PieceType.Knight,
                _ => PieceType.Queen // Default to queen
            };
        }
    }
}