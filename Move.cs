using System;

namespace Chess
{
    public class Move
    {
        public IPiece Piece { get; }
        public Position From { get; }
        public Position To { get; }
        public IPiece CapturedPiece { get; }
        public DateTime Timestamp { get; }

        public Move(IPiece piece, Position from, Position to, IPiece capturedPiece = null)
        {
            Piece = piece;
            From = from;
            To = to;
            CapturedPiece = capturedPiece;
            Timestamp = DateTime.Now;
        }

        public bool IsEnPassantMove()
        {
            return Piece is Pawn &&
                   Math.Abs(From.File - To.File) == 1 &&
                   CapturedPiece == null;
        }

        public override string ToString()
        {
            char fromFile = (char)('a' + From.File);
            char toFile = (char)('a' + To.File);
            int fromRank = 8 - From.Rank;
            int toRank = 8 - To.Rank;

            string moveNotation = $"{Piece.Name[0]}{fromFile}{fromRank}{(CapturedPiece != null ? "x" : "-")}{toFile}{toRank}";
            return moveNotation;
        }
    }
}