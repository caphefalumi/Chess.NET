using SplashKitSDK;

namespace Chess
{
    public class Bishop : Piece
    {
        public override PieceType Type => PieceType.Bishop;
        public override Player Color { get; }
        private static readonly Direction[] dirs = new Direction[]
        {
            Direction.UpLeft,
            Direction.UpRight,
            Direction.DownLeft,
            Direction.DownRight
        };
        public Bishop(Player color, Position pos, char pieceChar) : base(pieceChar)
        {
            Color = color;
            Position = pos;
        }
        public override Piece Copy()
        {
            Bishop copy = new Bishop(Color, Position, PieceChar);
            copy.HasMoved = HasMoved;
            return copy;
        }

        public override IEnumerable<Move> GetMoves(Board board)
        {
            return GenerateMoves(board, dirs).Select(to => new NormalMove(Position, to));
        }
    }
}