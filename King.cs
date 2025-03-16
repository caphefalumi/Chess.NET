using SplashKitSDK;

namespace Chess
{
    public class King : Piece
    {
        public override PieceType Type => PieceType.Rook;
        public override Player Color { get; }
        private static readonly Direction[] dirs = new Direction[]
        {
            Direction.Up,
            Direction.Right,
            Direction.Left,
            Direction.Down,
            Direction.UpLeft,
            Direction.UpRight,
            Direction.DownLeft,
            Direction.DownRight
        };
        public King(Player color, Position pos, char pieceChar) : base(pieceChar)
        {
            Color = color;
            Position = pos;
        }
        public override Piece Copy()
        {
            King copy = new King(Color, Position, PieceChar);
            copy.HasMoved = HasMoved;
            return copy;
        }
        private IEnumerable<Position> AdjacentMoves(Position from, Board board)
        {
            foreach (Direction dir in dirs)
            {
                Position to = from + dir;
                if (CanMoveTo(to, board))
                {
                    yield return to;
                }
            }
        }
        public override IEnumerable<Move> GetMoves(Position from, Board board) 
        {
            foreach (Position to in AdjacentMoves(from, board))
            {
                yield return new NormalMove(from, to);
            }
        }


    }
}