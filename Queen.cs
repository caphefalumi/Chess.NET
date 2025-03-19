namespace Chess
{
    public class Queen : Piece
    {
        public override PieceType Type => PieceType.Queen;
        public override Player Color { get; }

        private static readonly Direction[] dirs = new Direction[]
        {
            Direction.Up, Direction.Right, Direction.Left, Direction.Down,
            Direction.UpLeft, Direction.UpRight, Direction.DownLeft, Direction.DownRight
        };

        public Queen(Player color, Position pos, char pieceChar) : base(color, pieceChar)
        {
            Color = color;
            Position = pos;
        }

        public override Piece Copy()
        {
            Queen copy = new Queen(Color, Position, PieceChar);
            copy.HasMoved = HasMoved;
            return copy;
        }

        public override HashSet<Move> GetMoves(Board board)
        {
            HashSet<Move> moves = new HashSet<Move>();

            foreach (Position newPosition in GenerateMoves(board, dirs))
            {
                moves.Add(new NormalMove(Position, newPosition));
            }

            return moves;
        }
    }
}
