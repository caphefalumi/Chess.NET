using System.Numerics;

namespace Chess
{
    public abstract class SlidingPiece : Piece
    {
        public override abstract PieceType Type { get; }
        public override abstract Player Color { get; }
        public SlidingPiece(Player color, char pieceChar, Board board) : base(color, pieceChar, board)
        {}
        protected HashSet<Move> GenerateMove(Direction[] dirs)
        {
            HashSet<Move> moves = new HashSet<Move>();
            foreach (Direction dir in dirs)
            {
                for (Position to = Position + dir; Board.IsInside(to); to += dir)
                {
                    if (MyBoard.IsEmpty(to))
                    {
                        moves.Add(new NormalMove(Position, to));
                    }
                    else
                    {
                        Piece otherPiece = MyBoard.GetPieceAt(to);
                        if (otherPiece.Color != Color)
                        {
                            moves.Add(new NormalMove(Position, to));
                        }
                        break; // Stop after capturing or encountering a piece
                    }
                }
            }

            return moves;
        }

        protected HashSet<Move> GetSlidingMoves(Direction[] dirs)
        {
            return GenerateMove(dirs);
        }
    }
}
