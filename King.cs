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
        private IEnumerable<Position> AdjacentMoves(Board board)
        {
            foreach (Direction dir in dirs)
            {
                Position to = Position + dir;
                if (CanMoveTo(to, board))
                {
                    yield return to;
                }
            }
        }
        public override IEnumerable<Move> GetMoves(Board board) 
        {
            foreach (Position to in AdjacentMoves(board))
            {
                yield return new NormalMove(Position, to);
            }
        }

        public override bool CanCaptureOpponentKing(Board board)
        {
            return AdjacentMoves(board).Any(move =>
            {
                Piece piece = board.GetPieceAt(move);
                return piece != null && piece.Type == PieceType.King;
            });
        }

    }
}