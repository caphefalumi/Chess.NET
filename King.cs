namespace Chess
{
    public class King : Piece
    {
        public bool Castled { get; set; }

        private static readonly Direction[] dirs =
        {
            Direction.Up, Direction.Right, Direction.Left, Direction.Down,
            Direction.UpLeft, Direction.UpRight, Direction.DownLeft, Direction.DownRight
        };

        public King(char pieceChar, Position pos, Board board) : base(pieceChar, board)
        {
            Position = pos;
            Castled = false;
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
            HashSet<Move> moves = new HashSet<Move>();
            
            foreach (Direction dir in dirs)
            {
                Position newPos = Position + dir;
                if (CanMoveTo(newPos))
                {
                    moves.Add(new NormalMove(Position, newPos, this));
                }
            }

            // Only check castling if king hasn't moved and isn't in check
            if (!HasMoved && !Castled && !MyBoard.IsInCheck(Color))
            {
                if (CanCastleKS())
                {
                    moves.Add(new CastleMove(MoveType.CastleKS, Position, this));
                }

                if (CanCastleQS())
                {
                    moves.Add(new CastleMove(MoveType.CastleQS, Position, this));
                }
            }

            return moves;
        }

        
        public override IEnumerable<Move> GetAttackedSquares()
        {
            HashSet<Move> moves = new HashSet<Move>();
            
            foreach (Direction dir in dirs)
            {
                Position newPos = Position + dir;
                if (Board.IsInside(newPos))
                {
                    moves.Add(new NormalMove(Position, newPos, this));
                }
            }

            return moves;
        }
    }
}
