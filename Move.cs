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
        public abstract void Execute(Board board);
        public abstract void Undo(Board board);  // New method for rollback

    }
}