namespace Chess
{
    public class EnPassantMove : Move
    {
        public override MoveType Type => MoveType.EnPassant;
        public override Position From { get; }
        public override Position To { get; }
        public override Piece MovedPiece { get; }
        private Piece _capturedPiece;
        public override Piece CapturedPiece { get; set; }
        public override Sound Sound { get; protected set; }
        public EnPassantMove(Position from, Position to, Pawn pawn)
        {
            From = from;
            To = to;
            MovedPiece = pawn;
            Sound = Sounds.Capture;
        }
        public override void Execute(Board board, bool isSimulation)
        {
            Pawn pawn = board.GetPieceAt(From) as Pawn;
            _capturedPiece = board.GetPieceAt(To - pawn.Dir);  // Save captured piece (if any)

            board.Pieces.Remove(_capturedPiece);
            pawn.Position = To;
        }

        public override void Undo(Board board, bool isSimulation)
        {
            Pawn pawn = board.GetPieceAt(To) as Pawn;
            if (pawn is not null)
            {
                pawn.Position = From;
            }
            board.Pieces.Add(_capturedPiece);  // Restore captured piece
        }
    }
}
