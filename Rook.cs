namespace Chess
{
    public class Rook : SlidingPiece
    {
        private static readonly Direction[] dirs = new Direction[]
        {
            Direction.Up, Direction.Right, Direction.Left, Direction.Down
        };

        public Rook(char pieceChar, Position pos, Board board) : base(pieceChar, board)
        {
            Position = pos;
        }

        public override HashSet<Move> GetMoves()
        {
            return GetSlidingMoves(dirs);
        }
    }
}
