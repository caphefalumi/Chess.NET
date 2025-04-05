using SplashKitSDK;
using System.Numerics;
using System.Text;
using System.Linq;

namespace Chess
{
    public class Board : IDrawable
    {
        private HashSet<Piece> _pieces;
        private HashSet<Circle> _boardHighlights;
        private Rectangle[] _backgroundOverlays;
        private static Board _instance;
        private BoardRenderer _boardDrawer;
        private int _squareSize;
        private King _whiteKing;
        private King _blackKing;
        
        public bool IsGameOver { get; set; }
        public Sound CurrentSound { get; set; }
        public bool IsFlipped { get; set; }
        public HashSet<Piece> Pieces
        {
            get => _pieces;
            private set => _pieces = value;
        }
        public HashSet<Circle> BoardHighlights
        {
            get => _boardHighlights;
            set => _boardHighlights = value;
        }
        public Rectangle[] BackgroundOverlays
        {
            get => _backgroundOverlays;
            set => _backgroundOverlays = value;
        }
        public int SquareSize => _squareSize;
        public GameResult GameResult { get; set; }
        public MatchState MatchState { get; set; }

        private Board(int squareSize, int startX, int startY, Color lightColor, Color darkColor)
        {
            _squareSize = squareSize;
            _pieces = PieceFactory.CreatePieces(this);
            _boardHighlights = new HashSet<Circle>();
            _backgroundOverlays = new Rectangle[3];
            _boardDrawer = BoardRenderer.GetInstance(squareSize, startX, startY, lightColor, darkColor);
            _whiteKing = FindKing(Player.White);
            _blackKing = FindKing(Player.Black);
            GameResult = GameResult.Ongoing;
        }
        public static Board GetInstance(int squareSize, int startX, int startY, Color lightColor, Color darkColor)
        {
            if (_instance is null)
            {
                _instance = new Board(squareSize, startX, startY, lightColor, darkColor);
            }
            return _instance;
        }

        public void Draw()
        {
            _boardDrawer.Draw();
            _boardDrawer.Draw(_backgroundOverlays);
            _boardDrawer.Draw(_pieces);
            _boardDrawer.Draw(_boardHighlights);
        }

        public Piece GetPieceAt(int rank, int file)
        {
            foreach (Piece piece in Pieces)
            {
                if (piece.Position.Rank == rank && piece.Position.File == file)
                {
                    return piece;
                }
            }
            return null;
        }
        public Piece GetPieceAt(Position pos)
        {
            foreach (Piece piece in Pieces)
            {
                if (piece.Position.Equals(pos))
                {
                    return piece;
                }
            }
            return null;
        }
        public static bool IsInside(Position pos)
        {
            return pos.File >= 0 && pos.File < 8 && pos.Rank >= 0 && pos.Rank < 8;
        }

        public bool IsEmpty(Position pos)
        {
            return GetPieceAt(pos) is null;
        }

        private King FindKing(Player player)
        {
            return (King)_pieces.FirstOrDefault(piece => piece is King && piece.Color == player);
        }

        public bool IsInCheck(Player player)
        {
            King king = player == Player.White ? _whiteKing : _blackKing;
            return _pieces
                .Where(piece => piece.Color == player.Opponent())
                .SelectMany(piece => piece.GetAttackedSquares())
                .Any(move => move.To == king.Position);
        }

     

        public HashSet<Move> GetAllyMoves(Player player)
        {
            HashSet<Piece> piecesCopy = _pieces.ToHashSet();
            HashSet<Move> moves = piecesCopy
                .Where(piece => piece.Color == player)
                .SelectMany(piece => piece.GetLegalMoves())
                .ToHashSet();


            return moves;
        }

        public string GetFen()
        {
            StringBuilder fen = new StringBuilder();

            // 1. Board Representation (ranks 8 to 1, files a to h)
            for (int rank = 0; rank < 8; rank++)  // Start from rank 0 (which corresponds to the bottom in FEN)
            {
                int emptyCount = 0;
                for (int file = 0; file < 8; file++)
                {
                    Position pos = new Position(file, rank);  // Use the rank as it is, no reverse needed here
                    Piece piece = GetPieceAt(pos);

                    if (piece is null)
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
                        fen.Append(piece.PieceChar);
                    }
                }
                if (emptyCount > 0) fen.Append(emptyCount);
                if (rank < 7) fen.Append('/');  // Append slash except for the last rank
            }

            // 2. Active Color
            fen.Append(' ').Append(MatchState.CurrentPlayer == Player.White ? "w" : "b");

            // 3. Castling Availability
            StringBuilder castling = new StringBuilder();
            if (MatchState.CanWhiteCastleKingside) castling.Append('K');
            if (MatchState.CanWhiteCastleQueenside) castling.Append('Q');
            if (MatchState.CanBlackCastleKingside) castling.Append('k');
            if (MatchState.CanBlackCastleQueenside) castling.Append('q');
            fen.Append(' ').Append(castling.Length > 0 ? castling.ToString() : "-");

            // 4. En Passant Target Square
            fen.Append(' ');
            fen.Append('-');

            // 5. Halfmove Clock (moves since last capture or pawn advance)
            fen.Append(' ').Append(MatchState.HalfmoveClock);

            // 6. Fullmove Number
            fen.Append(' ').Append(MatchState.FullmoveNumber);

            return fen.ToString();
        }

        public void Flip()
        {
            // Flip all pieces' positions
            foreach (Piece piece in _pieces)
            {
                // Calculate new position (flip vertically)
                int newRank = 7 - piece.Position.Rank;
                piece.Position = new Position(piece.Position.File, newRank);
            }

            // Update kings' positions
            _whiteKing = FindKing(Player.White);
            _blackKing = FindKing(Player.Black);

            // Toggle the flipped state
            IsFlipped = !IsFlipped;

            // Update pawn directions after flipping
            foreach (Piece piece in _pieces)
            {
                if (piece is Pawn pawn)
                {
                    pawn.UpdateDirection();
                }
            }
        }
        
        public void ResetBoard()
        {
            // Reset pieces to initial position
            _pieces = PieceFactory.CreatePieces(this);
            
            // Clear highlights and overlays
            _boardHighlights.Clear();
            _backgroundOverlays = new Rectangle[3];
            
            // Reset kings reference
            _whiteKing = FindKing(Player.White);
            _blackKing = FindKing(Player.Black);
            
            // Reset game state
            GameResult = GameResult.Ongoing;
            IsGameOver = false;
            
            // Recreate MatchState with default settings
            if (MatchState != null)
            {
                MatchState = MatchState.GetInstance(this, Player.White);
            }
        }

        public void LoadFen(string fen)
        {
            if (string.IsNullOrWhiteSpace(fen)) return;

            _pieces.Clear(); // Clear current pieces

            string[] parts = fen.Split(' ');
            if (parts.Length < 2)
            {
                Console.WriteLine("Invalid FEN: Missing position or player turn");
                return;
            }

            string position = parts[0];
            string activeColor = parts[1];

            int rank = 0; // Start from rank 7 (top) for black pieces
            int file = 0;

            foreach (char c in position)
            {
                if (c == '/')
                {
                    rank++; // Move to next row *up* (decreasing rank)
                    file = 0;
                    continue;
                }

                if (char.IsDigit(c))
                {
                    file += c - '0'; // Empty squares
                    continue;
                }

                if (file > 7 || rank < 0 || rank > 7)
                {
                    Console.WriteLine($"Invalid board index at character '{c}' (file={file}, rank={rank})");
                    continue;
                }

                Position pos = new Position(file, rank); // (x, y)
                Piece piece = PieceFactory.CreatePiece(c, pos, this);

                if (piece != null)
                {
                    _pieces.Add(piece);

                    if (piece is King king)
                    {
                        if (piece.Color == Player.White)
                            _whiteKing = king;
                        else
                            _blackKing = king;
                    }
                }
                else
                {
                    Console.WriteLine($"Warning: Unrecognized FEN character '{c}' at {file},{rank}");
                }

                file++;
            }

            Player startingPlayer = activeColor == "w" ? Player.White : Player.Black;
            MatchState.GetInstance(this, startingPlayer);
        }
    }
}