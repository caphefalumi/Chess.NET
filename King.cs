using SplashKitSDK;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Chess
{
    public class King : Piece
    {
        public override PieceType Type => PieceType.King;
        public override Player Color { get; }
        public bool Castled { get; set; }


        private static readonly Direction[] dirs = new Direction[]
        {
            Direction.Up, Direction.Right, Direction.Left, Direction.Down,
            Direction.UpLeft, Direction.UpRight, Direction.DownLeft, Direction.DownRight
        };

        public King(Player color, Position pos, char pieceChar, Board board) : base(color, pieceChar, board)
        {
            Color = color;
            Position = pos;
        }


        private HashSet<Move> GetAdjacentMoves()
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


        private static bool IsRookHasMoved(Piece rook, Board board)
        {
            return rook is not null && rook.Type == PieceType.Rook && rook.HasMoved;
        }
        private static bool NoPiecesBetween(Position[] positions, Board board)
        {
           foreach (Position pos in positions)
           {
                if (!board.IsEmpty(pos))
                {
                    return false;
                }
           }
            return true;
        }
        private bool AreSquaresControlledByOpponent(Position[] positions)
        {
            return MyBoard.Pieces
                .Where(piece => piece.Color != this.Color) // Opponent pieces
                .SelectMany(piece => piece.GetAttackedSquares()) // Get their moves
                .Any(move => positions.Contains(move.To)); // Check if they attack these squares
        }

        private bool CanCastle(int rookFile, Position[] betweenPositions)
        {
            if (HasMoved) return false;

            Piece rook = MyBoard.GetPieceAt(rookFile, Position.Rank);
            if (IsRookHasMoved(rook, MyBoard)) return false;

            if (!NoPiecesBetween(betweenPositions, MyBoard)) return false;

            if (AreSquaresControlledByOpponent(betweenPositions)) return false;

            return true;
        }

        private bool CanCastleKS()
        {
            Position[] betweenPositions = { new Position(6, Position.Rank), new Position(5, Position.Rank) };
            return CanCastle(7, betweenPositions);
        }

        private bool CanCastleQS()
        {
            Position[] betweenPositions = { new Position(1, Position.Rank), new Position(2, Position.Rank), new Position(3, Position.Rank) };
            return CanCastle(0, betweenPositions);
        }


        public override HashSet<Move> GetMoves()
        {
            HashSet<Move> moves = new HashSet<Move>();
            moves.UnionWith(GetAdjacentMoves());

            if (CanCastleKS())
            {
                moves.Add(new CastleMove(MoveType.CastleKS, Position));
            }

            if (CanCastleQS())
            {
                moves.Add(new CastleMove(MoveType.CastleQS, Position));
            }

            return moves;
        }
    }
}
