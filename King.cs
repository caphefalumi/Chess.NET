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
        private bool _isCheckingAttacks = false;
        public bool Castled { get; set; }

        private static readonly Direction[] dirs = new Direction[]
        {
            Direction.Up, Direction.Right, Direction.Left, Direction.Down,
            Direction.UpLeft, Direction.UpRight, Direction.DownLeft, Direction.DownRight
        };

        public King(char pieceChar, Position pos, Board board) : base(pieceChar, board)
        {
            Position = pos;
            Castled = false;
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

            // Check if any of the squares the king moves through are under attack
            foreach (Position pos in betweenPositions)
            {
                if (IsSquareUnderAttack(pos))
                    return false;
            }

            return true;
        }

        private bool IsSquareUnderAttack(Position pos)
        {
            foreach (Piece piece in MyBoard.Pieces)
            {
                if (piece.Color != Color)
                {
                    if (piece.GetAttackedSquares().Any(move => move.To.Equals(pos)))
                        return true;
                }
            }
            return false;
        }

        public bool CanCastleKS()
        {
            Position[] betweenPositions = { new Position(5, Position.Rank), new Position(6, Position.Rank) };
            return CanCastle(7, betweenPositions);
        }

        public bool CanCastleQS()
        {
            Position[] betweenPositions = { new Position(1, Position.Rank), new Position(2, Position.Rank), new Position(3, Position.Rank) };
            return CanCastle(0, betweenPositions);
        }

        public override HashSet<Move> GetMoves()
        {
            var moves = new HashSet<Move>();
            
            // Normal king moves
            foreach (var offset in new[] { 
                (-1,-1), (-1,0), (-1,1),
                (0,-1),          (0,1),
                (1,-1),  (1,0),  (1,1) 
            })
            {
                Position newPos = new(Position.File + offset.Item1, Position.Rank + offset.Item2);
                if (Board.IsInside(newPos))
                {
                    var pieceAtDest = MyBoard.GetPieceAt(newPos);
                    if (pieceAtDest == null || pieceAtDest.Color != Color)
                    {
                        moves.Add(new NormalMove(Position, newPos, this));
                    }
                }
            }

            // Only check castling if king hasn't moved and isn't in check
            if (!HasMoved && !Castled && !_isCheckingAttacks && !MyBoard.IsInCheck(Color))
            {
                // Kingside castling
                if (CanCastleKingside())
                {
                    moves.Add(new CastleMove(MoveType.CastleKS, Position, this));
                }

                // Queenside castling
                if (CanCastleQueenside())
                {
                    moves.Add(new CastleMove(MoveType.CastleQS, Position, this));
                }
            }

            return moves;
        }

        private bool CanCastleKingside()
        {
            int rank = Color == Player.White ? 0 : 7;
            
            // Check rook position and status
            var rookPos = new Position(7, rank);
            var rook = MyBoard.GetPieceAt(rookPos) as Rook;
            if (rook == null || rook.HasMoved)
                return false;

            // Check if squares between king and rook are empty
            if (MyBoard.GetPieceAt(new Position(5, rank)) != null ||
                MyBoard.GetPieceAt(new Position(6, rank)) != null)
                return false;

            // Check if squares king moves through are safe
            if (IsSquareAttacked(new Position(4, rank)) ||
                IsSquareAttacked(new Position(5, rank)) ||
                IsSquareAttacked(new Position(6, rank)))
                return false;

            return true;
        }

        private bool CanCastleQueenside()
        {
            int rank = Color == Player.White ? 0 : 7;
            
            // Check rook position and status
            var rookPos = new Position(0, rank);
            var rook = MyBoard.GetPieceAt(rookPos) as Rook;
            if (rook == null || rook.HasMoved)
                return false;

            // Check if squares between king and rook are empty
            if (MyBoard.GetPieceAt(new Position(1, rank)) != null ||
                MyBoard.GetPieceAt(new Position(2, rank)) != null ||
                MyBoard.GetPieceAt(new Position(3, rank)) != null)
                return false;

            // Check if squares king moves through are safe
            if (IsSquareAttacked(new Position(4, rank)) ||
                IsSquareAttacked(new Position(3, rank)) ||
                IsSquareAttacked(new Position(2, rank)))
                return false;

            return true;
        }

        private bool IsSquareAttacked(Position pos)
        {
            if (_isCheckingAttacks)
                return false;

            _isCheckingAttacks = true;
            var isAttacked = MyBoard.Pieces
                .Where(p => p.Color != Color)
                .Any(p => p.GetAttackedSquares().Any(move => move.To.Equals(pos)));
            _isCheckingAttacks = false;
            
            return isAttacked;
        }

        public override IEnumerable<Move> GetAttackedSquares()
        {
            var moves = new HashSet<Move>();
            
            // Only include basic king moves for attacked squares
            foreach (var offset in new[] { 
                (-1,-1), (-1,0), (-1,1),
                (0,-1),          (0,1),
                (1,-1),  (1,0),  (1,1) 
            })
            {
                Position newPos = new(Position.File + offset.Item1, Position.Rank + offset.Item2);
                if (Board.IsInside(newPos))
                {
                    moves.Add(new NormalMove(Position, newPos, this));
                }
            }

            return moves;
        }
    }
}
