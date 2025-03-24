using SplashKitSDK;
using System.Collections.Generic;
using System.Linq;

namespace Chess
{
    public class Pawn : Piece
    {
        public override PieceType Type => PieceType.Pawn;
        public override Player Color { get; }
        private readonly Direction forward;

        public Pawn(Player color, Position pos, char pieceChar, Board board) : base(color, pieceChar, board)
        {
            Color = color;
            Position = pos;
            forward = (Color == Player.White) ? Direction.Up : Direction.Down;
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

        private HashSet<Move> ForwardMoves()
        {
            HashSet<Move> moves = new HashSet<Move>();

            Position singleMovePos = Position + forward;
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

                Position doubleMovePos = singleMovePos + forward;
                if (!HasMoved && CanMoveTo(doubleMovePos))
                {
                    moves.Add(new NormalMove(Position, doubleMovePos));
                }
            }

            return moves;
        }

        protected bool CanCaptureAt(Position pos)
        {
            return Board.IsInside(pos) && !MyBoard.IsEmpty(pos) && MyBoard.GetPieceAt(pos).Color != Color;
        }

        private HashSet<Move> CaptureMoves()
        {
            HashSet<Move> moves = new HashSet<Move>();

            foreach (Direction dir in new Direction[] { Direction.Left, Direction.Right })
            {
                Position to = Position + forward + dir;
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
            moves.UnionWith(ForwardMoves());
            moves.UnionWith(CaptureMoves());
            return moves;
        }
    }
}
