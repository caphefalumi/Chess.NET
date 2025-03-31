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
        public bool Castled { get; set; }


        private static readonly Direction[] dirs = new Direction[]
        {
            Direction.Up, Direction.Right, Direction.Left, Direction.Down,
            Direction.UpLeft, Direction.UpRight, Direction.DownLeft, Direction.DownRight
        };

        public King(char pieceChar, Position pos, Board board) : base(pieceChar, board)
        {
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
                    moves.Add(new NormalMove(Position, to, this));
                }
            }
            return moves;
        }


        private static bool IsRookHasMoved(Piece rook, Board board)
        {
            return rook is not null && rook is Rook && rook.HasMoved;
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

        private bool CanCastle(int rookFile, Position[] betweenPositions)
        {
            if (HasMoved) return false;

            Piece rook = MyBoard.GetPieceAt(rookFile, Position.Rank);
            if (IsRookHasMoved(rook, MyBoard)) return false;

            if (!NoPiecesBetween(betweenPositions, MyBoard)) return false;


            return true;
        }

        public bool CanCastleKS()
        {
            Position[] betweenPositions = { new Position(6, Position.Rank), new Position(5, Position.Rank) };
            return CanCastle(7, betweenPositions);
        }

        public bool CanCastleQS()
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
                Console.WriteLine("OK");
                moves.Add(new CastleMove(MoveType.CastleKS, Position, this));
            }

            if (CanCastleQS())
            {
                moves.Add(new CastleMove(MoveType.CastleQS, Position, this));
            }

            return moves;
        }
    }
}
