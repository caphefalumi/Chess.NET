namespace Chess
{
    public class Pawn : Piece
    {
        public Direction Dir { get; }
        public bool CanBeEnpassant { get; set; }
        private Position _originalPosition;
        public Pawn(char pieceChar, Position pos, Board board) : base(pieceChar, board)
        {
            Position = pos;
            _originalPosition = pos;
            Dir = (Color == Player.White) ? Direction.Up : Direction.Down;
            CanBeEnpassant = false;
        }

        private static HashSet<Move> PromotionMoves(Position from, Position to)
        {
            return new HashSet<Move>
            {
                new PromotionMove(from, to, PieceType.Queen),
                new PromotionMove(from, to, PieceType.Rook),
                new PromotionMove(from, to, PieceType.Bishop),
                new PromotionMove(from, to, PieceType.Knight)
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
                if (MyBoard.IsEmpty(doubleMovePos) && Position == _originalPosition)
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
