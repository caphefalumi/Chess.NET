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
        private Pawn _originalPawn;
        private Piece _promotedPiece;
        private Piece _capturedPiece;
        public PromotionMove(Position from, Position to, PieceType newType)
        {
            From = from;
            To = to;
            _newType = newType;
        }

        private Piece CreatePromotionPiece(Pawn pawn)
        {
            return PieceFactory.CreatePiece(pawn.PieceChar, To, pawn.MyBoard);
        }

        public override void Execute(Board board, bool isSimulation)
        {
            _originalPawn = board.GetPieceAt(From) as Pawn;
            board.Pieces.Remove(_originalPawn);
            _capturedPiece = board.GetPieceAt(To);
            if (_capturedPiece != null)
            {
                board.Pieces.Remove(_capturedPiece);
            }
            _promotedPiece = CreatePromotionPiece(_originalPawn);
            _promotedPiece.Position = To;
            _promotedPiece.HasMoved = true;
            board.CurrentSound = Sounds.Promote;
            board.Pieces.Add(_promotedPiece);
        }

        public override void Undo(Board board, bool isSimulation)
        {
            board.Pieces.Remove(_promotedPiece);
            _originalPawn.Position = From;
            board.Pieces.Add(_originalPawn);
            board.Pieces.Add(_capturedPiece);
        }
    }
}
