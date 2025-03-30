namespace Chess
{
    public class DoublePawnMove : Move
    {
        public override MoveType Type => MoveType.DoublePawn;
        public override Position From { get; }
        public override Position To { get; }
        public override Piece MovedPiece { get; }
        private Pawn _movedPawn { get; set; }
        public DoublePawnMove(Position from, Position to, Pawn pawn)
        {
            From = from;
            To = to;
            _movedPawn = pawn;
        }

        public override void Execute(Board board, bool isSimulation)
        {
            if (_movedPawn is Pawn)
            {
                _movedPawn.CanBeEnpassant = true;
            }

            board.CurrentSound = Sounds.MoveSelf;
            _movedPawn.Position = To;
            _movedPawn.HasMoved = true;
        }
        public override void Undo(Board board, bool isSimulation)
        {
            _movedPawn.Position = From;
            _movedPawn.HasMoved = false;

        }
    }
}
