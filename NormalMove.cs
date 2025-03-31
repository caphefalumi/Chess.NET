namespace Chess
{
    class NormalMove : Move, IMove
    {
        public override MoveType Type => MoveType.Normal;
        public override Position From { get; }
        public override Position To { get; }
        public override Piece MovedPiece { get; }
        public override Piece CapturedPiece { get; set; }

        public NormalMove(Position from, Position to, Piece piece)
        {
            From = from;
            To = to;
            MovedPiece = piece;
        }

        public override void Execute(Board board, bool isSimulation)
        {
            CapturedPiece = board.GetPieceAt(To);  // Save captured piece (if any)

            if (CapturedPiece != null)
            {
                board.Pieces.Remove(CapturedPiece);
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
            if (CapturedPiece is not null)
            {
                board.Pieces.Add(CapturedPiece);  // Restore captured piece
            }
        }
    }
}
