using System;

namespace Chess
{
    public class CastleMove : Move
    {
        public override MoveType Type { get; }
        public override Position From { get; }
        public override Position To { get; }

        private Position rookFrom;
        private Position rookTo;
        private readonly Direction kingMoveDir;

        public CastleMove(MoveType type, Position kingPos)
        {
            Type = type;
            From = kingPos;

            if (type == MoveType.CastleKS)
            {
                kingMoveDir = Direction.Right;
                To = new Position(kingPos.File, 6);
                rookFrom = new Position(kingPos.File, 7);

                rookTo = new Position(kingPos.File, 5);
            }
            else if (type == MoveType.CastleQS)
            {
                kingMoveDir = Direction.Left;
                To = new Position(kingPos.File, 2);
                rookFrom = new Position(kingPos.File, 0);
                rookTo = new Position(kingPos.File, 3);
            }
        }

        public override void Execute(Board board)
        {
            // Move the king
            new NormalMove(From, To).Execute(board);
            // Move the rook
            new NormalMove(rookFrom, rookTo).Execute(board);
        }

        public override void Undo(Board board)
        {
            new NormalMove(To, From).Execute(board);
            new NormalMove(rookFrom, rookTo).Execute(board);
        }
    }
}
