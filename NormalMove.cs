using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    class NormalMove : Move
    {
        public override MoveType Type => MoveType.Normal;
        public override Position From { get; }
        public override Position To { get; }
        public NormalMove(Position from, Position to)
        {
            From = from;
            To = to;
        }
        public override void Execute(Board board)
        {
            Piece piece = board.GetPieceAt(From);
            Piece capturedPiece = board.GetPieceAt(To);
            if (capturedPiece != null)
            {
                board.Pieces.Remove(capturedPiece);
            }
            piece.Position = To;
            piece.HasMoved = true;
        }
    }
}
