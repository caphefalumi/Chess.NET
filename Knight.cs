using SplashKitSDK;

namespace Chess
{
    public class Knight : Piece
    {
        public override PieceType Type => PieceType.Knight;
        public override Player Color { get; }
        public Knight(Player color, Position pos, char pieceChar) : base(pieceChar)
        {
            Color = color;
            Position = pos;
        }
        public override Piece Copy()
        {
            Knight copy = new Knight(Color, Position, PieceChar);
            copy.HasMoved = HasMoved;
            return copy;
        }

        private IEnumerable<Position> LShapedMoves(Position from, Board board)
        {
            foreach (Direction file in new Direction[] { Direction.Up, Direction.Down })
            {
                foreach (Direction rank in new Direction[] { Direction.Left, Direction.Right })
                {
                    Position leftPos = from + 2 * file + rank;
                    Position rightPos = from + 2 * rank + file;

                    if (CanMoveTo(leftPos, board) && board[leftPos].Color != Color)
                    {
                        yield return leftPos;
                    }
                    if (CanMoveTo(rightPos, board))
                    {
                        yield return rightPos;
                    }
                }
            }
        }
        public override IEnumerable<Move> GetMoves(Position from, Board board)
        {
            return LShapedMoves(from, board).Select(to => new NormalMove(from, to));
        }
    }
}