
namespace Chess
{
    public static class PieceFactory
    {
        private static readonly char[,] boardSetup =
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

        public static HashSet<Piece> CreatePieces(Board board, bool isReversed = false)
        {
            HashSet<Piece> pieces = new HashSet<Piece>();
            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    char pieceChar = boardSetup[rank, file];
                    if (pieceChar != '.')
                    {
                        Position position = new Position(file, rank);
                        pieces.Add(CreatePiece(pieceChar, position, board));
                    }
                }
            }

            return pieces;
        }

        public static Piece CreatePiece(Char pieceChar, Position position, Board board)
        {
            switch (char.ToUpper(pieceChar))
            {
                case 'P': return new Pawn(pieceChar, position, board);
                case 'R': return new Rook(pieceChar, position, board);
                case 'N': return new Knight(pieceChar, position, board);
                case 'B': return new Bishop(pieceChar, position, board);
                case 'Q': return new Queen(pieceChar, position, board);
                case 'K': return new King(pieceChar, position, board);
                default: throw new ArgumentException("Invalid PieceType" + pieceChar);
            }
        }

        public static char GetPieceChar(PieceType type, Player color)
        {
            char c = type switch
            {
                PieceType.Pawn => 'p',
                PieceType.Rook => 'r',
                PieceType.Knight => 'n',
                PieceType.Bishop => 'b',
                PieceType.Queen => 'q',
                PieceType.King => 'k',
                _ => throw new ArgumentException($"Invalid piece type: {type}")
            };
            return color == Player.White ? char.ToUpper(c) : c;
        }

        public static PieceType GetPieceType(char c)
        {
            return char.ToLower(c) switch
            {
                'p' => PieceType.Pawn,
                'r' => PieceType.Rook,
                'n' => PieceType.Knight,
                'b' => PieceType.Bishop,
                'q' => PieceType.Queen,
                'k' => PieceType.King,
                _ => throw new ArgumentException($"Invalid piece character: {c}")
            };
        }
    }
}
