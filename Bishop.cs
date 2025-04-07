namespace Chess
{
    public class Bishop : SlidingPiece
    {
        private static readonly Direction[] dirs =
        {
            Direction.UpLeft, Direction.UpRight, Direction.DownLeft, Direction.DownRight
        };

        public Bishop(char pieceChar, Position pos, Board board) : base(pieceChar, board)
        {
            Position = pos;
        }
        
        public override HashSet<Move> GetMoves()
        {
            return GetSlidingMoves(dirs);
        }
    }
}
