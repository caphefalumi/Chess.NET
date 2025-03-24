using SplashKitSDK;
using System.Collections.Generic;
using System.Linq;

namespace Chess
{
    public class Knight : Piece
    {
        public override PieceType Type => PieceType.Knight;
        public override Player Color { get; }

        private static readonly Direction[] dirs = 
        {
            Direction.Up + Direction.UpLeft, Direction.Up + Direction.UpRight,
            Direction.Down + Direction.DownLeft, Direction.Down + Direction.DownRight,
            Direction.Left + Direction.UpLeft , Direction.Left + Direction.DownLeft,
            Direction.Right + Direction.UpRight, Direction.Right + Direction.DownRight
        };
        public Knight(Player color, Position pos, char pieceChar, Board board) : base(color, pieceChar, board)
        {
            Color = color;
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
                    moves.Add(new NormalMove(Position, to));
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
