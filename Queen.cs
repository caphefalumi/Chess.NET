namespace Chess
{
    public class Queen : SlidingPiece
    {
        public override PieceType Type => PieceType.Queen;
        public override Player Color { get; }

        private static readonly Direction[] dirs = new Direction[]
        {
            Direction.Up, Direction.Right, Direction.Left, Direction.Down,
            Direction.UpLeft, Direction.UpRight, Direction.DownLeft, Direction.DownRight
        };

        public Queen(Player color, Position pos, char pieceChar, Board board) : base(color, pieceChar, board)
        {
            Color = color;
            Position = pos;
        }


        public override HashSet<Move> GetMoves()
        {
            return GetSlidingMoves(dirs);
        }
    }
}
