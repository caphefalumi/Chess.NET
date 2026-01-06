using Chess.Core;
using Chess.Pieces;
using Chess.Interfaces;
using Chess.UI.Media;

namespace Chess.Moves
{
    class NormalMove : Move, IMove
    {
        private readonly Position _from;
        private readonly Position _to;
        private readonly Piece _movedPiece;

        private Piece _capturedPiece;
        private Sound _sound;

        public override MoveType Type => MoveType.Normal;
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

        public NormalMove(Position from, Position to, Piece piece)
        {
            _from = from;
            _to = to;
            _movedPiece = piece;
            _sound = Sounds.MoveSelf;
        }

        public override void Execute(Board board, bool isSimulation)
        {
            _capturedPiece = board.GetPieceAt(_to);  // Save captured piece (if any)

            if (_capturedPiece != null)
            {
                board.Pieces.Remove(_capturedPiece);
                _sound = Sounds.Capture;
            }

            _movedPiece.Position = _to;

            if (!isSimulation)
            {
                _movedPiece.HasMoved = true;
            }
        }

        public override void Undo(Board board, bool isSimulation)
        {
            if (_movedPiece is not null)
            {
                _movedPiece.Position = _from;
                if (!isSimulation)
                {
                    _movedPiece.HasMoved = false;
                }
            }

            if (_capturedPiece is not null)
            {
                board.Pieces.Add(_capturedPiece);  // Restore captured piece
            }
        }
    }
}
