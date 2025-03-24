namespace Chess
{
    class NormalMove : Move, IMove
    {
        public override MoveType Type => MoveType.Normal;
        public override Position From { get; }
        public override Position To { get; }
        private Piece _capturedPiece;  // Store captured piece

        public NormalMove(Position from, Position to)
        {
            From = from;
            To = to;
        }

        public override void Execute(Board board)
        {
            Piece piece = board.GetPieceAt(From);
            _capturedPiece = board.GetPieceAt(To);  // Save captured piece (if any)

            if (_capturedPiece != null)
            {
                board.Pieces.Remove(_capturedPiece);
                board.CurrentSound = Sounds.Capture;
            }
            else
            {
                board.CurrentSound = Sounds.MoveSelf;
            }
            piece.Position = To;
            piece.HasMoved = true;
        }

        public override void Undo(Board board)
        {
            Piece piece = board.GetPieceAt(To);
            piece.Position = From;
            piece.HasMoved = false;  // Optional: Reset HasMoved

            if (_capturedPiece != null)
            {
                board.Pieces.Add(_capturedPiece);  // Restore captured piece
            }
        }
    }
}
