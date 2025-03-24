namespace Chess
{
    public class EnPassantMove : Move
    {
        public override MoveType Type => MoveType.Normal;
        public override Position From { get; }
        public override Position To { get; }
        private Piece _capturedPiece;  // Store captured piece
        
        public EnPassantMove(Position from, Position to)
        {
            From = from;
            To = to;
        }
        public override void Execute(Board board)
        {
            Pawn pawn = board.GetPieceAt(From) as Pawn;
            _capturedPiece = board.GetPieceAt(To - pawn.Dir);  // Save captured piece (if any)

            board.Pieces.Remove(_capturedPiece);
            board.CurrentSound = Sounds.Capture;
            pawn.Position = To;
        }

        public override void Undo(Board board)
        {
            Pawn pawn = board.GetPieceAt(To) as Pawn;
            pawn.Position = From;
            board.Pieces.Add(_capturedPiece);  // Restore captured piece
        }
    }
}
