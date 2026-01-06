using Chess.Core;
using Chess.Pieces;
using Chess.Interfaces;
using Chess.UI.Media;

namespace Chess.Moves
{
    public class CastleMove : Move, IMove
    {
        private MoveType _type;
        private Position _from;
        private Position _to;
        private Piece _movedPiece;
        private Piece _capturedPiece;
        private Sound _sound;

        private Rook _rook;
        private Position _rookFrom;
        private Position _rookTo;

        public override MoveType Type => _type;
        public override Position From => _from;
        public override Position To => _to;
        public override Piece MovedPiece => _movedPiece;

        public override Piece CapturedPiece
        {
            get => _capturedPiece;
            set => _capturedPiece = value;
        }

        public override Sound Sound
        {
            get => _sound;
            protected set => _sound = value;
        }

        public CastleMove(MoveType type, Position kingPos, King king)
        {
            _type = type;
            _from = kingPos;
            _movedPiece = king;

            if (type == MoveType.CastleKS)
            {
                _to = new Position(6, kingPos.Rank);
                _rookFrom = new Position(7, kingPos.Rank);
                _rookTo = new Position(5, kingPos.Rank);
            }
            else if (type == MoveType.CastleQS)
            {
                _to = new Position(2, kingPos.Rank);
                _rookFrom = new Position(0, kingPos.Rank);
                _rookTo = new Position(3, kingPos.Rank);
            }

            _sound = Sounds.Castle;
        }

        public override void Execute(Board board, bool isSimulation)
        {
            King king = (King)_movedPiece;
            _rook = (Rook)board.GetPieceAt(_rookFrom);

            king.Position = _to;
            if (_rook != null)
            {
                _rook.Position = _rookTo;
            }

            if (!isSimulation)
            {
                king.HasMoved = true;
                _rook.HasMoved = true;
                king.Castled = true;
            }
        }

        public override void Undo(Board board, bool isSimulation)
        {
            King king = (King)_movedPiece;

            if (king != null)
            {
                king.Position = _from;
                if (!isSimulation)
                {
                    king.HasMoved = false;
                    king.Castled = false;
                }
            }

            if (_rook != null)
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
