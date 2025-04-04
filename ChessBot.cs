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
            return ConvertNotationToMove(moveNotation);
        }
        private async Task<Move> GetStockfishMove(string fen, int thinkingTimeMs)
        {
            string moveNotation = await _stockfishBot.GetBestMoveAsync(fen, thinkingTimeMs);
            return ConvertNotationToMove(moveNotation);
        }
        public Move ConvertNotationToMove(string moveNotation)
        {
            string sourceStr = moveNotation.Substring(0, 2);
            string destStr = moveNotation.Substring(2, 2);
            
            Position source = new Position(sourceStr);
            Position destination = new Position(destStr);

            // Get the piece at the source position
            Piece piece = _board.GetPieceAt(source);
            if (piece == null) return null;

            // Check for promotion
            if (moveNotation.Length == 5)
            {
                char promotionPiece = moveNotation[4];
                return new PromotionMove(source, destination, piece, PieceFactory.GetPieceType(promotionPiece));
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

    }
}
