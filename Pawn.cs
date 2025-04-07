namespace Chess
{
    public class Pawn : Piece
    {
        public Direction Dir { get; private set; }
        public bool CanBeEnpassant { get; set; }
        private Position _originalPosition;
        
        public Pawn(char pieceChar, Position pos, Board board) : base(pieceChar, board)
        {
            Position = pos;
            _originalPosition = pos;
            UpdateDirection();
            CanBeEnpassant = false;
        }

        public void UpdateDirection()
        {
            // White pawns always move toward rank 7 (top)
            // Black pawns always move toward rank 0 (bottom)
            if (Color == Player.White)
            {
                Dir = Direction.Up;
                if (MyBoard.IsFlipped)
                {
                    Dir = Direction.Down;
                    _originalPosition = new Position(_originalPosition.File, 7 - _originalPosition.Rank);
                }
            }
            else
            {
                Dir = Direction.Down;
                if (MyBoard.IsFlipped)
                {
                    Dir = Direction.Up;
                    _originalPosition = new Position(_originalPosition.File, 7 - _originalPosition.Rank);
                }
            }
        }

        private HashSet<Move> PromotionMoves(Position from, Position to)
        {
            return new HashSet<Move>
            {
                new PromotionMove(from, to, this, PieceType.Queen),
                new PromotionMove(from, to, this, PieceType.Rook),
                new PromotionMove(from, to, this, PieceType.Bishop),
                new PromotionMove(from, to, this, PieceType.Knight)
            };
        }

        private HashSet<Move> DirMoves()
        {
            HashSet<Move> moves = new HashSet<Move>();

            Position singleMovePos = Position + Dir;
            if (MyBoard.IsEmpty(singleMovePos))
            {
                if (singleMovePos.Rank == 0 || singleMovePos.Rank == 7)
                {
                    moves.UnionWith(PromotionMoves(Position, singleMovePos));
                }
                else
                {
                    moves.Add(new NormalMove(Position, singleMovePos, this));
                }

                Position doubleMovePos = singleMovePos + Dir;
                if (MyBoard.IsEmpty(doubleMovePos) && HasMoved == false)
                {
                    moves.Add(new DoublePawnMove(Position, doubleMovePos, this));
                }
            }

            return moves;
        }

        private bool CanCaptureAt(Position pos)
        {
            Piece piece = MyBoard.GetPieceAt(pos);

            return piece is not null && piece.Color != Color ;
        }

        private HashSet<Move> CaptureMoves()
        {
            HashSet<Move> moves = new HashSet<Move>();

            foreach (Direction direction in new Direction[] { Direction.Left, Direction.Right })
            {
                Position to = Position + Dir + direction;
                if (CanCaptureAt(to))
                {
                    if (to.Rank == 0 || to.Rank == 7)
                    {
                        moves.UnionWith(PromotionMoves(Position, to));
                    }
                    else
                    {
                        moves.Add(new NormalMove(Position, to, this));
                    }
                }
                Position enPassantCapture = Position + direction;
                Pawn pawn = MyBoard.GetPieceAt(enPassantCapture) as Pawn;
                if (pawn is not null && pawn.CanBeEnpassant && pawn.Color != Color && ((pawn.Color == Player.White && pawn.Position.Rank == 4) || (pawn.Color == Player.Black && pawn.Position.Rank == 3))) 
                {
                    moves.Add(new EnPassantMove(Position, enPassantCapture + Dir, this));
                }
            }

            return moves;
        }
        public override HashSet<Move> GetAttackedSquares()
        {
            return CaptureMoves();
        }

        public override HashSet<Move> GetMoves()
        {
            HashSet<Move> moves = new HashSet<Move>();
            moves.UnionWith(DirMoves());
            moves.UnionWith(CaptureMoves());
            return moves;
        }



    }
}
