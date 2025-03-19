using SplashKitSDK;
using System.Collections.Generic;
using System.Linq;

namespace Chess
{
    public class Bishop : Piece
    {
        public override PieceType Type => PieceType.Bishop;
        public override Player Color { get; }

        private static readonly Direction[] dirs = new Direction[]
        {
            Direction.UpLeft, Direction.UpRight, Direction.DownLeft, Direction.DownRight
        };

        public Bishop(Player color, Position pos, char pieceChar) : base(color, pieceChar)
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
        public override HashSet<Move> GetMoves(Board board)
        {
            HashSet<Move> moves = new HashSet<Move>();
            foreach (Position to in GenerateMoves(board, dirs))
            {
                moves.Add(new NormalMove(Position, to));
            }
            return moves;
        }
    }
}
