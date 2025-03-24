using SplashKitSDK;
using System.Collections.Generic;
using System.Linq;

namespace Chess
{
    public class Pawn : Piece
    {
        public Direction Dir { get; }

        public Pawn(char pieceChar, Position pos, Board board) : base(pieceChar, board)
        {
            Position = pos;
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
                    moves.Add(new NormalMove(Position, singleMovePos));
                }

                Position doubleMovePos = singleMovePos + Dir;
                if (!HasMoved && MyBoard.IsEmpty(doubleMovePos))
                {
                    moves.Add(new DoublePawnMove(Position, doubleMovePos));
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
                        moves.Add(new NormalMove(Position, to));
                    }
                }
                Position enPassantCapture = Position + direction;
                Pawn pawn = MyBoard.GetPieceAt(enPassantCapture) as Pawn;
                if (pawn is not null && pawn.CanBeEnpassant)
                {
                    moves.Add(new EnPassantMove(Position, enPassantCapture + Dir));
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
