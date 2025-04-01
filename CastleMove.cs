using System;

namespace Chess
{
    public class CastleMove : Move, IMove
    {
        public override MoveType Type { get; }
        public override Position From { get; }
        public override Position To { get; }
        public override Piece MovedPiece { get; }
        public override Piece CapturedPiece { get; set; }
        private Rook _rook;
        private Position _rookFrom;
        private Position _rookTo;

        public CastleMove(MoveType type, Position kingPos, King king)
        {
            Type = type;
            From = kingPos;
            MovedPiece = king;

            if (type == MoveType.CastleKS)
            {
                To = new Position(6, kingPos.Rank);
                _rookFrom = new Position(7, kingPos.Rank);
                _rookTo = new Position(5, kingPos.Rank);
            }
            else if (type == MoveType.CastleQS)
            {
                To = new Position(2, kingPos.Rank);
                _rookFrom = new Position(0, kingPos.Rank);
                _rookTo = new Position(3, kingPos.Rank);
            }
        }

        public override void Execute(Board board, bool isSimulation)
        {
            King king = (King)MovedPiece;
            _rook = (Rook)board.GetPieceAt(_rookFrom);
            
            // Move the king
            king.Position = To;
            // Move the rook
            _rook.Position = _rookTo;
            
            if (!isSimulation)
            {
                king.HasMoved = true;
                _rook.HasMoved = true;
                king.Castled = true;
                board.CurrentSound = Sounds.Castle;
            }
        }

        public override void Undo(Board board, bool isSimulation)
        {
            King king = (King)MovedPiece;
            
            if (king is not null)
            {
                king.Position = From;
                if (!isSimulation)
                {
                    king.HasMoved = false;
                    king.Castled = false;
                }
            }
            
            if (_rook is not null)
            {
                _rook.Position = _rookFrom;
                if (!isSimulation)
                {
                    _rook.HasMoved = false;
                }
            }
        }
    }
}