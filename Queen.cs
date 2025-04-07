namespace Chess
{
    public class Queen : SlidingPiece
    {
        private static readonly Direction[] dirs =
        {
            Direction.Up, Direction.Right, Direction.Left, Direction.Down,
            Direction.UpLeft, Direction.UpRight, Direction.DownLeft, Direction.DownRight
        };

        public Queen(char pieceChar, Position pos, Board board) : base(pieceChar, board)
        {
            Position = pos;
        }


        public override HashSet<Move> GetMoves()
        {
            return GetSlidingMoves(dirs);
        }
    }
}
