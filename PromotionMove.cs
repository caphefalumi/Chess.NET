namespace Chess
{
    public class PromotionMove : Move, IMove
    {
        public override MoveType Type => MoveType.Promotion;
        public override Position From { get; }
        public override Position To { get; }
        private readonly PieceType _newType;
        private Piece _originalPawn;
        private Piece _promotedPiece;

        public PromotionMove(Position from, Position to, PieceType newType)
        {
            From = from;
            To = to;
            _newType = newType;
        }

        private Piece CreatePromotionPiece(Piece piece)
        {
            return PieceFactory.CreatePiece(_newType, piece.Color, To, piece.MyBoard);
        }

        public override void Execute(Board board)
        {
            _originalPawn = board.GetPieceAt(From);
            board.Pieces.Remove(_originalPawn);
            Piece capturedPiece = board.GetPieceAt(To);
            if (capturedPiece != null)
            {
                board.Pieces.Remove(capturedPiece);
            }
            _promotedPiece = CreatePromotionPiece(_originalPawn);
            _promotedPiece.Position = To;
            _promotedPiece.HasMoved = true;
            board.CurrentSound = Sounds.Promote;
            board.Pieces.Add(_promotedPiece);
        }

        public override void Undo(Board board)
        {
            board.Pieces.Remove(_promotedPiece);
            _originalPawn.Position = From;
            board.Pieces.Add(_originalPawn);
        }
    }
}
