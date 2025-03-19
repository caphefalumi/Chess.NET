using SplashKitSDK;
using System.Collections.Generic;
using System.Linq;

namespace Chess
{
    public class Knight : Piece
    {
        public override PieceType Type => PieceType.Knight;
        public override Player Color { get; }

        public Knight(Player color, Position pos, char pieceChar) : base(color, pieceChar)
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

        private HashSet<Position> LShapedMoves(Board board)
        {
            HashSet<Position> moves = new HashSet<Position>();

            foreach (Direction file in new Direction[] { Direction.Up, Direction.Down })
            {
                foreach (Direction rank in new Direction[] { Direction.Left, Direction.Right })
                {
                    Position leftPos = Position + 2 * file + rank;
                    Position rightPos = Position + 2 * rank + file;

                    if (CanMoveTo(leftPos, board))
                    {
                        moves.Add(leftPos);
                    }
                    if (CanMoveTo(rightPos, board))
                    {
                        moves.Add(rightPos);
                    }
                }
            }

            return moves;
        }

        public override HashSet<Move> GetMoves(Board board)
        {
            HashSet<Move> moves = new HashSet<Move>();

            foreach (Position to in LShapedMoves(board))
            {
                moves.Add(new NormalMove(Position, to));
            }

            return moves;
        }
    }
}
