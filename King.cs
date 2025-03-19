using SplashKitSDK;
using System.Collections.Generic;
using System.Linq;

namespace Chess
{
    public class King : Piece
    {
        public override PieceType Type => PieceType.King;
        public override Player Color { get; }

        private static readonly Direction[] dirs = new Direction[]
        {
            Direction.Up, Direction.Right, Direction.Left, Direction.Down,
            Direction.UpLeft, Direction.UpRight, Direction.DownLeft, Direction.DownRight
        };

        public King(Player color, Position pos, char pieceChar) : base(color, pieceChar)
        {
            Color = color;
            Position = pos;
        }

        public override Piece Copy()
        {
            King copy = new King(Color, Position, PieceChar);
            copy.HasMoved = HasMoved;
            return copy;
        }

        private HashSet<Position> AdjacentMoves(Board board)
        {
            HashSet<Position> moves = new HashSet<Position>();

            foreach (Direction dir in dirs)
            {
                Position to = Position + dir;
                if (CanMoveTo(to, board))
                {
                    moves.Add(to);
                }
            }
            return moves;
        }

        private static bool IsRookHasMoved(Piece rook, Board board)
        {
            if (board.IsEmpty(new Position(7, 7)))
            {
                return false;
            }
            return rook.Type == PieceType.Rook && rook.HasMoved;
        }
        private static bool NoPiecesBetween(IEnumerable<Position> positions, Board board)
        {
           return positions.All(pos => board.IsEmpty(pos));
        }

        private bool CanCastleKS(Board board)
        {
            if (HasMoved)
            {
                return false;
            }
            Piece rook = board.GetPieceAt(Position.Rank, 7);
            Position[] betweenPositions = { new Position(Position.Rank, 7), new Position(Position.Rank, 6) };

            return !IsRookHasMoved(rook, board);
        }
        private bool CanCastleQS(Board board)
        {
            if (HasMoved)
            {
                return false;
            }
            Piece rook = board.GetPieceAt(Position.Rank, 0);
            Position[] betweenPositions = { new Position(Position.Rank, 1), new Position(Position.Rank, 2), new Position  (Position.Rank, 3) };

            return !IsRookHasMoved(rook, board) && NoPiecesBetween(betweenPositions, board);
        }

        public override HashSet<Move> GetMoves(Board board)
        {
            HashSet<Move> moves = new HashSet<Move>();

            foreach (Position to in AdjacentMoves(board))
            {
                moves.Add(new NormalMove(Position, to));
            }

            if (CanCastleKS(board))
            {
                Console.WriteLine("OKKKKK");
                moves.Add(new CastleMove(MoveType.CastleKS, Position));
            }

            if (CanCastleQS(board))
            {
                moves.Add(new CastleMove(MoveType.CastleQS, Position));
            }

            return moves;
        }
    }
}
