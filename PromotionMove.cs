namespace Chess
{
    public class PromotionMove : Move, IMove
    {
        private readonly Position _from;
        private readonly Position _to;
        private readonly Piece _movedPiece;
        private readonly PieceType _newType;
        private Piece _capturedPiece;
        private Piece _promotedPiece;
        private Sound _sound;

        public override MoveType Type => MoveType.Promotion;
        public override Position From => _from;
        public override Position To => _to;
        public override Piece MovedPiece => _movedPiece;
        public override Piece CapturedPiece
        {
            get => _capturedPiece;
            set => _capturedPiece = value;
        }
        public PieceType NewPieceType => _newType;
        public override Sound Sound
        {
            get => _sound;
            protected set => _sound = value;
        }

        public PromotionMove(Position from, Position to, Piece piece, PieceType newType)
        {
            _from = from;
            _to = to;
            _movedPiece = piece;
            _newType = newType;
            _sound = Sounds.Promote;
        }

        private Piece CreatePromotionPiece(PieceType newType)
        {
            return PieceFactory.CreatePiece(
                PieceFactory.GetPieceChar(newType, _movedPiece.Color),
                _to,
                _movedPiece.MyBoard);
        }

        public override void Execute(Board board, bool isSimulation)
        {
            board.Pieces.Remove(_movedPiece);
            _capturedPiece = board.GetPieceAt(_to);
            if (_capturedPiece != null)
            {
                board.Pieces.Remove(_capturedPiece);
            }
            _promotedPiece = CreatePromotionPiece(_newType);
            _promotedPiece.Position = _to;
            _promotedPiece.HasMoved = true;
            board.Pieces.Add(_promotedPiece);
        }

        public override void Undo(Board board, bool isSimulation)
        {
            board.Pieces.Remove(_promotedPiece);
            _movedPiece.Position = _from;
            board.Pieces.Add(_movedPiece);
            if (_capturedPiece != null)
            {
                board.Pieces.Add(_capturedPiece);
            }
        }
    }
}
