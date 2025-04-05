namespace Chess
{
    public class ChessBot
    {
        private static ChessBot _instance;
        private readonly Board _board;
        private readonly IBot _apiBot;
        private readonly IBot _stockfishBot;
        private const int DEFAULT_THINKING_TIME = 1000;
        private Player _player;

        private ChessBot(Board board, Player player)
        {
            _board = board;
            _apiBot = new ChessApiBot();
            _stockfishBot = new StockfishBot();
            _player = player;
        }

        public static ChessBot GetInstance(Board board, Player player)
        {
            if (_instance == null)
            {
                _instance = new ChessBot(board, player);
            }
            return _instance;
        }

        public async Task<Move> GetBestMove(int thinkingTimeMs = DEFAULT_THINKING_TIME)
        {
            string fen = _board.GetFen();
            
            if (await _apiBot.IsAvailableAsync())
            {
                return await GetApiMove(fen, thinkingTimeMs);
            }
            else if (await _stockfishBot.IsAvailableAsync())
            {
                return await GetStockfishMove(fen, thinkingTimeMs);
            }
            else
            {
                Console.WriteLine("No internet connection or Stockfish is not available");
                return _board.GetAllyMoves(_player).FirstOrDefault();
            }
        }
        private async Task<Move> GetApiMove(string fen, int thinkingTimeMs)
        {
            string moveNotation = await _apiBot.GetBestMoveAsync(fen, thinkingTimeMs);
            return Move.ConvertNotation(moveNotation, _board);
        }
        private async Task<Move> GetStockfishMove(string fen, int thinkingTimeMs)
        {
            string moveNotation = await _stockfishBot.GetBestMoveAsync(fen, thinkingTimeMs);
            return Move.ConvertNotation(moveNotation, _board);
        }


    }
}
