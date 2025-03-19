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

        public Pawn(Player color, Position pos, char pieceChar) : base(color, pieceChar)
        {
            Color = color;
            Position = pos;
            forward = (Color == Player.White) ? Direction.Up : Direction.Down;
        }

        public override Piece Copy()
        {
            Pawn copy = new Pawn(Color, Position, PieceChar);
            copy.HasMoved = HasMoved;
            return copy;
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

        private HashSet<Move> ForwardMoves(Position from, Board board)
        {
            HashSet<Move> moves = new HashSet<Move>();

            Position singleMovePos = from + forward;
            if (board.IsEmpty(singleMovePos))
            {
                if (singleMovePos.Rank == 0 || singleMovePos.Rank == 7)
                {
                    Console.WriteLine("PROMOTE");
                    moves.UnionWith(PromotionMoves(from, singleMovePos));
                }
                else
                {
                    moves.Add(new NormalMove(from, singleMovePos));
                }

                Position doubleMovePos = singleMovePos + forward;
                if (!HasMoved && CanMoveTo(doubleMovePos, board))
                {
                    moves.Add(new NormalMove(from, doubleMovePos));
                }
            }

            return moves;
        }

        protected bool CanCaptureAt(Position pos, Board board)
        {
            return Board.IsInside(pos) && !board.IsEmpty(pos) && board.GetPieceAt(pos).Color != Color;
        }

        private HashSet<Move> CaptureMoves(Position from, Board board)
        {
            HashSet<Move> moves = new HashSet<Move>();

            foreach (Direction dir in new Direction[] { Direction.Left, Direction.Right })
            {
                Position to = from + forward + dir;
                if (CanCaptureAt(to, board))
                {
                    if (to.Rank == 0 || to.Rank == 7)
                    {
                        Console.WriteLine("PROMOTE");
                        moves.UnionWith(PromotionMoves(from, to));
                    }
                    else
                    {
                        moves.Add(new NormalMove(from, to));
                    }
                }
            }

            return moves;
        }

        public override HashSet<Move> GetMoves(Board board)
        {
            HashSet<Move> moves = new HashSet<Move>();
            moves.UnionWith(ForwardMoves(Position, board));
            moves.UnionWith(CaptureMoves(Position, board));
            return moves;
        }
    }
}
