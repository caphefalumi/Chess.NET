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
                        Player color = char.IsUpper(pieceChar) ? Player.White : Player.Black;
                        PieceType type = GetPieceType(pieceChar);
                        pieces.Add(CreatePiece(type, color, position));
                    }
                }
            }

            return pieces;
        }

        public static Piece CreatePiece(PieceType type, Player color, Position position)
        {
            switch (type)
            {
                case PieceType.Pawn: return new Pawn(color, position, 'p');
                case PieceType.Rook: return new Rook(color, position, 'r');
                case PieceType.Knight: return new Knight(color, position, 'n');
                case PieceType.Bishop: return new Bishop(color, position, 'b');
                case PieceType.Queen: return new Queen(color, position, 'q');
                case PieceType.King: return new King(color, position, 'k');
                default: throw new ArgumentException("Invalid PieceType" + type);
            }
        }

        private static PieceType GetPieceType(char pieceChar)
        {
            switch (char.ToUpper(pieceChar))
            {
                case 'P': return PieceType.Pawn;
                case 'R': return PieceType.Rook;
                case 'N': return PieceType.Knight;
                case 'B': return PieceType.Bishop;
                case 'Q': return PieceType.Queen;
                case 'K': return PieceType.King;
                default: throw new ArgumentException("Invalid piece character" + pieceChar);
            }
        }
    }
}
