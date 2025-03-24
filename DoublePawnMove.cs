using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    public class DoublePawnMove : Move
    {
        public override MoveType Type => MoveType.DoublePawn;
        public override Position From { get; }
        public override Position To { get; }
        public DoublePawnMove(Position from, Position to)
        {
            From = from;
            To = to;
        }

        public override void Execute(Board board)
        {
            Piece piece = board.GetPieceAt(From);
            if (piece is Pawn)
            {
                piece.CanBeEnpassant = true;
            }

            board.CurrentSound = Sounds.MoveSelf;
            piece.Position = To;
            piece.HasMoved = true;
        }
        public override void Undo(Board board)
        {
            new NormalMove(From, To).Undo(board);
        }
    }
}
