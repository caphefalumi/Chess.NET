using System.Collections.Generic;
using System.Text;
using SplashKitSDK;

namespace Chess
{
    public class Board
    {
        private static Board _instance; // Singleton instance
        private IPiece _selectedPiece; // Track the selected piece
        BoardDrawer _boardDrawer;

        public static HashSet<IPiece> Pieces;
        public static HashSet<IShape> Shapes;

        // Add move history tracking
        public static List<Move> MoveHistory { get; private set; } = new List<Move>();
        public static Move LastMove => MoveHistory.Count > 0 ? MoveHistory[MoveHistory.Count - 1] : null;


        private Board(int squareSize, int startX, int startY, Color lightColor, Color darkColor)
        {
            Pieces = PieceFactory.CreatePieces();
            Shapes = new HashSet<IShape>();
            MoveHistory = new List<Move>();
            _boardDrawer = BoardDrawer.GetInstance(squareSize, startX, startY, lightColor, darkColor);
        }

        public static Board GetInstance(int squareSize, int startX, int startY, Color lightColor, Color darkColor)
        {
            if (_instance is null)
            {
                _instance = new Board(squareSize, startX, startY, lightColor, darkColor);
            }
            return _instance;
        }

        public static Board GetInstance()
        {
            if (_instance is null)
            {
                _instance = new Board(32, 0, 0, Color.LightGray, Color.DarkGray);
            }
            return _instance;
        }

        public static Piece FindPieceAt(Position pos)
        {
            foreach (Piece piece in Pieces)
            {
                if (piece.Position == pos)
                {
                    return piece;
                }
            }
            return null;
        }

        public void Draw()
        {
            _boardDrawer.Draw();
            _boardDrawer.Draw(Shapes);
            _boardDrawer.Draw(Pieces);
        }

        // Add a method to record moves
        public static void RecordMove(IPiece piece, Position from, Position to, IPiece capturedPiece = null)
        {
            Move move = new Move(piece, from, to, capturedPiece);
            MoveHistory.Add(move);


            // Check if this is a pawn moving two squares
            if (piece is Pawn pawn && Math.Abs(from.Rank - to.Rank) == 2)
            {
                pawn.CanBeEnPassantCaptured = true;
            }
        }

        // Add method to get current FEN position
        public string GetFEN()
        {
            StringBuilder fen = new StringBuilder();

            // 1. Piece placement
            for (int rank = 0; rank < 8; rank++)
            {
                int emptyCount = 0;

                for (int file = 0; file < 8; file++)
                {
                    IPiece piece = FindPieceAt(new Position(file, rank));

                    if (piece == null)
                    {
                        emptyCount++;
                    }
                    else
                    {
                        if (emptyCount > 0)
                        {
                            fen.Append(emptyCount);
                            emptyCount = 0;
                        }

                        char pieceChar = GetFENCharForPiece(piece);
                        fen.Append(pieceChar);
                    }
                }

                if (emptyCount > 0)
                {
                    fen.Append(emptyCount);
                }

                if (rank < 7)
                {
                    fen.Append('/');
                }
            }

            // 2. Active color
            string activeColor = MoveHistory.Count % 2 == 0 ? "w" : "b";
            fen.Append(" ").Append(activeColor);

            // 3. Castling availability
            string castlingRights = GetCastlingRights();
            fen.Append(" ").Append(castlingRights);

            // 4. En passant target square
            string enPassantTarget = GetEnPassantTarget();
            fen.Append(" ").Append(enPassantTarget);

            // 5. Halfmove clock (for 50-move rule)
            int halfmoveClock = GetHalfmoveClock();
            fen.Append(" ").Append(halfmoveClock);

            // 6. Fullmove number
            int fullmoveNumber = (MoveHistory.Count / 2) + 1;
            fen.Append(" ").Append(fullmoveNumber);

            return fen.ToString();
        }

        private char GetFENCharForPiece(IPiece piece)
        {
            char pieceChar = ' ';

            switch (piece.Name)
            {
                case "Pawn": pieceChar = 'p'; break;
                case "Knight": pieceChar = 'n'; break;
                case "Bishop": pieceChar = 'b'; break;
                case "Rook": pieceChar = 'r'; break;
                case "Queen": pieceChar = 'q'; break;
                case "King": pieceChar = 'k'; break;
            }

            if (piece.Color == "White")
            {
                pieceChar = char.ToUpper(pieceChar);
            }

            return pieceChar;
        }

        private string GetCastlingRights()
        {
            bool whiteKingSide = false;
            bool whiteQueenSide = false;
            bool blackKingSide = false;
            bool blackQueenSide = false;

            IPiece whiteKing = null;
            IPiece blackKing = null;

            foreach (IPiece piece in Pieces)
            {
                if (piece.Name == "King" && piece.Color == "White")
                {
                    whiteKing = piece;
                }
                else if (piece.Name == "King" && piece.Color == "Black")
                {
                    blackKing = piece;
                }

                if (piece.Name == "Rook" && piece.Color == "White")
                {
                    if (piece.Position.File == 0) // Queen-side rook
                    {
                        if (!piece.HasMoved)
                        {
                            whiteQueenSide = true;
                        }
                    }
                    else if (piece.Position.File == 7) // King-side rook
                    {
                        if (!piece.HasMoved)
                        {
                            whiteKingSide = true;
                        }
                    }
                }
                else if (piece.Name == "Rook" && piece.Color == "Black")
                {
                    if (piece.Position.File == 0) // Queen-side rook
                    {
                        if (!piece.HasMoved)
                        {
                            blackQueenSide = true;
                        }
                    }
                    else if (piece.Position.File == 7) // King-side rook
                    {
                        if (!piece.HasMoved)
                        {
                            blackKingSide = true;
                        }
                    }
                }
            }

            // Check if kings have moved
            if (whiteKing != null && whiteKing.HasMoved)
            {
                whiteKingSide = whiteQueenSide = false;
            }

            if (blackKing != null && blackKing.HasMoved)
            {
                blackKingSide = blackQueenSide = false;
            }

            StringBuilder castlingRights = new StringBuilder();

            if (whiteKingSide) castlingRights.Append("K");
            if (whiteQueenSide) castlingRights.Append("Q");
            if (blackKingSide) castlingRights.Append("k");
            if (blackQueenSide) castlingRights.Append("q");

            if (castlingRights.Length == 0)
            {
                castlingRights.Append("-");
            }

            return castlingRights.ToString();
        }

        private string GetEnPassantTarget()
        {
            if (LastMove != null && LastMove.Piece is Pawn pawn && Math.Abs(LastMove.From.Rank - LastMove.To.Rank) == 2)
            {
                int enPassantRank = LastMove.From.Rank + (LastMove.To.Rank - LastMove.From.Rank) / 2;
                char fileChar = (char)('a' + LastMove.To.File);
                int rankNotation = 8 - enPassantRank;
                return $"{fileChar}{rankNotation}";
            }

            return "-";
        }

        private int GetHalfmoveClock()
        {
            int halfmoveClock = 0;

            // Count moves since last pawn move or capture
            for (int i = MoveHistory.Count - 1; i >= 0; i--)
            {
                Move move = MoveHistory[i];

                if (move.Piece is Pawn || move.CapturedPiece != null)
                {
                    break;
                }

                halfmoveClock++;
            }

            return halfmoveClock;
        }
    }
}