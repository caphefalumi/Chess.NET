using Chess.Core;
using Chess.Moves;

namespace Chess.Pieces
{
    public class Rook : SlidingPiece
    {
        private static readonly Direction[] dirs =
        {
            Direction.Up, Direction.Right, Direction.Left, Direction.Down
        };

        public Rook(char pieceChar, Position pos, Board board) : base(pieceChar, board)
        {
            Position = pos;
        }

        public override HashSet<Move> GetMoves()
        {
            return GetSlidingMoves(dirs);
        }
    }
}
