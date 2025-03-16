using System.Collections.Generic;

namespace Chess
{
    public static class PieceFactory
    {
        public static HashSet<Piece> CreatePieces()
        {
            HashSet<Piece> pieces = new HashSet<Piece>();

            char[,] boardSetup = new char[8, 8]
            {
                { 'r', 'n', 'b', 'q', 'k', 'b', 'n', 'r' },
                { 'p', 'p', 'p', 'p', 'p', 'p', 'p', 'p' },
                { '.', '.', '.', '.', '.', '.', '.', '.' },
                { '.', '.', '.', '.', '.', '.', '.', '.' },
                { '.', '.', '.', '.', '.', '.', '.', '.' },
                { '.', '.', '.', '.', '.', '.', '.', '.' },
                { 'P', 'P', 'P', 'P', 'P', 'P', 'P', 'P' },
                { 'R', 'N', 'B', 'Q', 'K', 'B', 'N', 'R' }
            };


            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    char pieceChar = boardSetup[rank, file];

                    if (pieceChar != '.')
                    {
                        Position position = new Position(file, rank);
                        Piece piece = CreatePiece(pieceChar, position);
                        pieces.Add(piece);
                    }
                }
            }

            return pieces;
        }

        private static Piece CreatePiece(char pieceChar, Position position)
        {
            Player color;
            if (char.IsUpper(pieceChar))
            {
                color = Player.White;
            }
            else
            {
                color = Player.Black;
            }

            switch (char.ToUpper(pieceChar))
            {
                case 'P': return new Pawn(color, position, pieceChar);
                case 'R': return new Rook(color, position, pieceChar);
                case 'N': return new Knight(color, position, pieceChar);
                case 'B': return new Bishop(color, position, pieceChar);
                case 'Q': return new Queen(color, position, pieceChar);
                case 'K': return new King(color, position, pieceChar);
                default: return null;
            }
        }
    }
}
