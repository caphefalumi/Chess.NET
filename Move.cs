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
        public abstract void Undo(Board board, bool isSimulation = false);  // New method for rollback
    }
}