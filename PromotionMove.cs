namespace Chess
{
    public class PromotionMove : Move, IMove
    {
        public override MoveType Type => MoveType.Promotion;
        public override Position From { get; }
        public override Position To { get; }
        public override Piece MovedPiece { get; }
        public override Piece CapturedPiece { get; set; }

        private readonly PieceType _newType;
        private Piece _promotedPiece;
        private Piece _capturedPiece;
        public PromotionMove(Position from, Position to, Piece piece,  PieceType newType)
        {
            From = from;
            To = to;
            MovedPiece = piece;
            _newType = newType;
        }

        private Piece CreatePromotionPiece(PieceType newType)
        {
            return PieceFactory.CreatePiece(PieceFactory.GetPieceChar(newType, MovedPiece.Color), To, MovedPiece.MyBoard);
        }

        public override void Execute(Board board, bool isSimulation)
        {
            board.Pieces.Remove(MovedPiece);
            _capturedPiece = board.GetPieceAt(To);
            if (_capturedPiece != null)
            {
                board.Pieces.Remove(_capturedPiece);
            }
            _promotedPiece = CreatePromotionPiece(_newType);
            _promotedPiece.Position = To;
            _promotedPiece.HasMoved = true;
            board.CurrentSound = Sounds.Promote;
            board.Pieces.Add(_promotedPiece);
        }

        public override void Undo(Board board, bool isSimulation)
        {
            board.Pieces.Remove(_promotedPiece);
            MovedPiece.Position = From;
            board.Pieces.Add(MovedPiece);
            if (_capturedPiece != null)
            {
                board.Pieces.Add(_capturedPiece);
            }
        }
    }
}
