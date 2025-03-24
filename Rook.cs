using SplashKitSDK;
using System.Collections.Generic;
using System.Linq;

namespace Chess
{
    public class Rook : SlidingPiece
    {
        public override PieceType Type => PieceType.Rook;
        public override Player Color { get; }


        private static readonly Direction[] dirs = new Direction[]
        {
            Direction.Up, Direction.Right, Direction.Left, Direction.Down
        };

        public Rook(Player color, Position pos, char pieceChar, Board board) : base(color, pieceChar, board)
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
