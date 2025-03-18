namespace Chess
{
    public abstract class Move
    {
        public abstract MoveType Type { get; }
        public abstract Position From { get; }
        public abstract Position To { get; }
        public abstract void Execute(Board board);

        public virtual bool IsLegal(Board board)
        {
            Player player = board.GetPieceAt(From).Color;
            Board boardCopy = board.Copy();
            Execute(boardCopy);
            return !boardCopy.IsInCheck(player);
        }
    }
}