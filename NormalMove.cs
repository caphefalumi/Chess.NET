namespace Chess
{
    class NormalMove : Move, IMove
    {
        public override MoveType Type => MoveType.Normal;
        public override Position From { get; }
        public override Position To { get; }
        public override Piece MovedPiece { get; }
        private Piece _capturedPiece;  // Store captured piece

        public NormalMove(Position from, Position to, Piece piece)
        {
            From = from;
            To = to;
            MovedPiece = piece;
        }

        public override void Execute(Board board, bool isSimulation)
        {
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
            MovedPiece.Position = To;
            if (!isSimulation)
            {
                MovedPiece.HasMoved = true;
            }
        }

        public override void Undo(Board board, bool isSimulation)
        {
            if (MovedPiece is not null)
            {
                MovedPiece.Position = From;
                if (!isSimulation)
                {
                    MovedPiece.HasMoved = false;
                }
            }
            if (_capturedPiece is not null)
            {
                board.Pieces.Add(_capturedPiece);  // Restore captured piece
            }
        }
    }
}
