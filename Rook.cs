using SplashKitSDK;
using System.Collections.Generic;
using System.Linq;

namespace Chess
{
    public class Rook : Piece
    {
        public override PieceType Type => PieceType.Rook;
        public override Player Color { get; }

        private static readonly Direction[] dirs = new Direction[]
        {
            Direction.Up, Direction.Right, Direction.Left, Direction.Down
        };

        public Rook(Player color, Position pos, char pieceChar) : base(color, pieceChar)
        {
            Color = color;
            Position = pos;
        }

        public override Piece Copy()
        {
            Rook copy = new Rook(Color, Position, PieceChar);
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
