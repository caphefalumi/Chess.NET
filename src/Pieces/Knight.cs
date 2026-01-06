using Chess.Core;
using Chess.Moves;

namespace Chess.Pieces
{
    public class Knight : Piece
    {
        private static readonly Direction[] dirs = 
        {
            Direction.Up + Direction.UpLeft, Direction.Up + Direction.UpRight,
            Direction.Down + Direction.DownLeft, Direction.Down + Direction.DownRight,
            Direction.Left + Direction.UpLeft , Direction.Left + Direction.DownLeft,
            Direction.Right + Direction.UpRight, Direction.Right + Direction.DownRight
        };
        public Knight(char pieceChar, Position pos, Board board) : base(pieceChar, board)
        {
            Position = pos;
        }

        private HashSet<Move> GetLShapedMoves()
        {
            HashSet<Move> moves = new HashSet<Move>();

            foreach (Direction dir in dirs)
            {
                Position to = Position + dir;
                if (CanMoveTo(to))
                {
                    moves.Add(new NormalMove(Position, to, this));
                }
            }
            return moves;
        }

        public override HashSet<Move> GetMoves()
        {
            return GetLShapedMoves();
        }

    }
}
