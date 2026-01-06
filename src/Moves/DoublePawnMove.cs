using Chess.Core;
using Chess.Pieces;
using Chess.Interfaces;
using Chess.UI.Media;

namespace Chess.Moves
{
    public class DoublePawnMove : Move
    {
        public override MoveType Type => MoveType.DoublePawn;
        public override Position From { get; }
        public override Position To { get; }
        public override Piece MovedPiece { get; }
        private Pawn _movedPawn { get; set; }
        public override Piece CapturedPiece { get; set; }
        public override Sound Sound { get; protected set; }
        public DoublePawnMove(Position from, Position to, Pawn pawn)
        {
            From = from;
            To = to;
            MovedPiece = pawn;
            _movedPawn = pawn;
            Sound = Sounds.MoveSelf;
        }

        public override void Execute(Board board, bool isSimulation)
        {
            if (_movedPawn is Pawn && !isSimulation)
            {
                _movedPawn.CanBeEnpassant = true;
            }

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
