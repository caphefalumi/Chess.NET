namespace Chess
{
    public abstract class Move
    {
        public abstract MoveType Type { get; }
        public abstract Position From { get; }
        public abstract Position To { get; }
        public abstract Piece MovedPiece { get; }
        public abstract Piece CapturedPiece { get; set; }
        public abstract void Execute(Board board, bool isSimulation = false);
        public abstract void Undo(Board board, bool isSimulation = false);

        public override string ToString()
        {
            string moveStr = $"{From}{To}";
            
            // Add promotion piece if it's a promotion move
            if (Type == MoveType.Promotion)
            {
                PromotionMove pMove = (PromotionMove)this;
                moveStr += PieceFactory.GetPieceChar(pMove.NewPieceType, pMove.MovedPiece.Color).ToString().ToLower();
            }
            
            return moveStr;
        }

        public static Move ConvertNotation(string moveNotation, Board board)
        {
            if (string.IsNullOrEmpty(moveNotation))
            {
                Console.WriteLine("[Move] Invalid move notation: null or empty");
                return null;
            }

            try
            {
                string sourceStr = moveNotation.Substring(0, 2);
                string destStr = moveNotation.Substring(2, 2);
                
                Position source = new Position(sourceStr);
                Position destination = new Position(destStr);

                // Get the piece at the source position
                Piece piece = board.GetPieceAt(source);
                if (piece == null)
                {
                    Console.WriteLine($"[Move] No piece found at position {sourceStr}");
                    return null;
                }

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
                if (piece is Pawn pawn && destination.File != source.File && board.GetPieceAt(destination) == null)
                {
                    return new EnPassantMove(source, destination, pawn);
                }

                // Default to normal move
                return new NormalMove(source, destination, piece);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Move] Error parsing move notation: {ex.Message}");
                return null;
            }
        }
    }
}