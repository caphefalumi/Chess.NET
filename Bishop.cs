using SplashKitSDK;
using System.Collections.Generic;
using System.Linq;

namespace Chess
{
    public class Bishop : SlidingPiece
    {
        public override PieceType Type => PieceType.Bishop;
        public override Player Color { get; }
        private static readonly Direction[] dirs =
        {
            Direction.UpLeft, Direction.UpRight, Direction.DownLeft, Direction.DownRight
        };

        public Bishop(Player color, Position pos, char pieceChar, Board board) : base(color, pieceChar, board)
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
