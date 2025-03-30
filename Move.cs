namespace Chess
{
    public enum MoveType
    {
        Normal,
        CastleKS,
        CastleQS,
        DoublePawn,
        EnPassant,
        Promotion
    }

    public abstract class Move
    {
        public abstract MoveType Type { get; }
        public abstract Position From { get; }
        public abstract Position To { get; }
        public abstract Piece MovedPiece { get; }
        public abstract void Execute(Board board, bool isSimulation = false);
        public abstract void Undo(Board board, bool isSimulation = false);  // New method for rollback
        public static bool operator ==(Move move1, Move move2)
        {
            return move1.From == move2.From && move1.To == move2.To && move1.MovedPiece == move2.MovedPiece;
        }
        public static bool operator !=(Move move1, Move move2)
        {
            return !(move1 == move2);
        }

    }
}