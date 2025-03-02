using System.Collections.Generic;

namespace Chess
{
    public static class PieceFactory
    {
        public static HashSet<IPiece> CreatePieces()
        {
            HashSet<IPiece> pieces = new HashSet<IPiece>();

            char[,] boardSetup = new char[8, 8]
            {
                { 'R', 'N', 'B', 'Q', 'K', 'B', 'N', 'R' },
                { 'P', 'P', 'P', 'P', 'P', 'P', 'P', 'P' },
                { '.', '.', '.', '.', '.', '.', '.', '.' },
                { '.', '.', '.', '.', '.', '.', '.', '.' },
                { '.', '.', '.', '.', '.', '.', '.', '.' },
                { '.', '.', '.', '.', '.', '.', '.', '.' },
                { 'p', 'p', 'p', 'p', 'p', 'p', 'p', 'p' },
                { 'r', 'n', 'b', 'q', 'k', 'b', 'n', 'r' }
            };

            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    char pieceChar = boardSetup[rank, file];

                    if (pieceChar != '.')
                    {
                        Position position = new Position(file, rank);
                        IPiece piece = CreatePiece(pieceChar, position);
                        pieces.Add(piece);
                    }
                }
            }
            return pieces;
        }

        private static IPiece CreatePiece(char pieceChar, Position position)
        {
            string color;
            if (char.IsUpper(pieceChar))
            {
                color = "White";
            }
            else
            {
                color = "Black";
            }

            switch (char.ToUpper(pieceChar))
            {
                case 'P': return new Pawn("Pawn", color, position);
                case 'R': return new Rook("Rook", color, position);
                case 'N': return new Knight("Night", color, position);
                case 'B': return new Bishop("Bishop", color, position);
                case 'Q': return new Queen("Queen", color, position);
                case 'K': return new King("King", color, position);
                default: return null;
            }
        }
    }
}
