using SplashKitSDK;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text;
namespace Chess
{
    public class Board : IDrawable
    {
        private HashSet<Piece> _pieces;
        private HashSet<Circle> _boardHighlights;
        private Rectangle[] _backgroundOverlays;
        private static Board _instance;
        private BoardDrawer _boardDrawer;
        private int _squareSize;
        private Color _lightColor;
        private Color _darkColor;
        public Sound CurrentSound { get; set; }

        public HashSet<Piece> Pieces
        {
            get => _pieces;
            set => _pieces = value;
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
        public int SquareSize
        {
            get => _squareSize;
        }
        public int PieceCounts
        {
            get => _pieces.Count;
        }

        public ulong Occupancy { get; set; }

        public GameState GameState { get; set; }

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
                if (piece.Position == pos)
                {
                    return piece;
                }
            }
            return null;
        }
        private Board(int squareSize, int startX, int startY, Color lightColor, Color darkColor)
        {
            _squareSize = squareSize;
            _lightColor = lightColor;
            _darkColor = darkColor;
            _pieces = PieceFactory.CreatePieces(this);
            _boardHighlights = new HashSet<Circle>();
            _backgroundOverlays = new Rectangle[3];
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

        public void Draw()
        {
            _boardDrawer.Draw();
            _boardDrawer.Draw(_backgroundOverlays);
            _boardDrawer.Draw(_pieces);
            _boardDrawer.Draw(_boardHighlights);
        }

        public static bool IsInside(Position pos)
        {
            return pos.File >= 0 && pos.File < 8 && pos.Rank >= 0 && pos.Rank < 8;
        }

        public bool IsEmpty(Position pos)
        {
            return GetPieceAt(pos) is null;
        }

        public IEnumerable<Position> PiecePositions(Player player)
        {
            return _pieces.Where(piece => piece.Color == player).Select(piece => piece.Position);
        }

        private Position FindKing(Player player)
        {
            return _pieces.FirstOrDefault(piece => piece is King && piece.Color == player)?.Position;
        }

        public King GetKing()
        {
            return _pieces.OfType<King>().FirstOrDefault(king => king.Color == GameState.CurrentPlayer);
        }

        public bool IsInCheck(Player player)
        {
            Position kingPos = FindKing(player);
            if (kingPos is null) return false; // Safety check in case king isn't found
            return _pieces
                .Where(piece => piece.Color == player.Opponent())
                .SelectMany(piece => piece.GetAttackedSquares())
                .Any(move => move.To == kingPos);
        }

        public HashSet<Move> GetAllyMoves(Player player)
        {
            var piecesCopy = _pieces.ToHashSet(); // Create a copy of the collection
            return piecesCopy
                .Where(piece => piece.Color == player)
                .SelectMany(piece => piece.GetLegalMoves())
                .ToHashSet();
        }
        public string GetFen()
        {
            StringBuilder fen = new StringBuilder();

            // 1️⃣ Board Representation
            for (int rank = 7; rank >= 0; rank--)
            {
                int emptyCount = 0;
                for (int file = 0; file < 8; file++)
                {
                    Position pos = new Position(file, rank);
                    Piece piece = GetPieceAt(pos); // Assuming Board has a method to get a piece at a position

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
                        fen.Append(piece.PieceChar); // Implement ToFenChar() in Piece class
                    }
                }
                if (emptyCount > 0) fen.Append(emptyCount);
                if (rank > 0) fen.Append('/');
            }

            fen.Append(" ").Append(GameState.CurrentPlayer == Player.White ? "w" : "b");

            fen.Append(" ");
            //string castling = GetCastlingRights(); // Implement GetCastlingRights()
            //fen.Append(string.IsNullOrEmpty(castling) ? "-" : castling);

            //fen.Append(" ").Append(GetEnPassantSquare()); // Implement GetEnPassantSquare()

            //fen.Append(" ").Append(halfmoveClock); // Track in Board

            //// 6️⃣ Fullmove Counter
            //fen.Append(" ").Append(fullmoveCounter); // Track in Board

            return fen.ToString();
        }
    }
}