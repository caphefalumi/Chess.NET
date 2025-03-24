using System;

namespace Chess
{
    public class CastleMove : Move, IMove
    {
        public override MoveType Type { get; }
        public override Position From { get; }
        public override Position To { get; }

        private Position _rookFrom;
        private Position _rookTo;
        private readonly Direction _kingMoveDir;

        public CastleMove(MoveType type, Position kingPos)
        {
            Type = type;
            From = kingPos;

            if (type == MoveType.CastleKS)
            {
                _kingMoveDir = Direction.Right;
                To = new Position(6, kingPos.Rank);
                _rookFrom = new Position(7, kingPos.Rank);
                _rookTo = new Position(5, kingPos.Rank);
            }
            else if (type == MoveType.CastleQS)
            {
                _kingMoveDir = Direction.Left;
                To = new Position(2, kingPos.Rank);
                _rookFrom = new Position(0, kingPos.Rank);
                _rookTo = new Position(3, kingPos.Rank);
            }
        }

        public override void Execute(Board board)
        {
            // Move the king
            King king = board.GetKing();
            Piece rook = board.GetPieceAt(_rookFrom);

            if (king is King && rook != null)
            {
                // Move the king
                king.Position = To;
                king.HasMoved = true;
                board.CurrentSound = Sounds.Castle;
                // Move the rook
                rook.Position = _rookTo;
                rook.HasMoved = true;
            }
        }

        public override void Undo(Board board)
        {
            King king = board.GetKing();
            Piece rook = board.GetPieceAt(_rookTo);
            
            if (king is not null)
            {
                king.Position = From;
                king.HasMoved = false;
                king.Castled = true;
            }
            
            if (rook is not null)
            {
                rook.Position = _rookFrom;
                rook.HasMoved = false;
            }
        }
    }
}