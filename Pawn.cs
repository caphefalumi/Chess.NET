using SplashKitSDK;
using System.Runtime.CompilerServices;

namespace Chess
{
    public class Pawn : Piece
    {
        public override PieceType Type => PieceType.Pawn;
        public override Player Color { get; }
        private readonly Direction forward;
        public Pawn(Player color, Position pos, char pieceChar) : base(pieceChar)
        {
            Color = color;
            Position = pos;
            if (Color == Player.White)
            {
                forward = Direction.Up;
            }
            else
            {
                forward = Direction.Down;
            }
        }
        public override Piece Copy()
        {
            Pawn copy = new Pawn(Color, Position, PieceChar);
            copy.HasMoved = HasMoved;
            return copy;
        }
        

        private IEnumerable<Move> ForwardMoves(Position from, Board board)
        {
            Position singleMovePos = from + forward;
            if (CanMoveTo(singleMovePos, board))
            {
                yield return new NormalMove(from, singleMovePos);
                Position doubleMovePos = singleMovePos + forward;
                if (!HasMoved && CanMoveTo(doubleMovePos, board))
                {
                    yield return new NormalMove(from, doubleMovePos);
                }
            }
        }
        protected bool CanCaptureAt(Position pos, Board board)
        {
            if (!Board.IsInside(pos) || board.IsEmpty(pos) || board[pos].Color == Color)
            {
                return false;
            }
            return true;
        }
        private IEnumerable<Move> CaptureMoves(Position from, Board board)
        {
            foreach (Direction dir in new Direction[] {Direction.Left, Direction.Right})
            {
                Position to = from + forward + dir;
                if (CanCaptureAt(to, board))
                {
                    yield return new NormalMove(from, to);
                }
            }
        }


        public override IEnumerable<Move> GetMoves(Position from, Board board)
        {
            return ForwardMoves(from, board).Concat(CaptureMoves(from, board));
        }


    }
}