using SplashKitSDK;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
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
            return _pieces.FirstOrDefault(piece => piece.Type == PieceType.King && piece.Color == player)?.Position;
        }

        public King GetKing()
        {
            return _pieces.OfType<King>().FirstOrDefault(king => king.Color == GameState.CurrentPlayer);
        }
        public bool IsInCheck(Player player)
        {
            Position kingPos = FindKing(player);
            if (kingPos is null) return false; // Safety check in case king isn't found
            Console.WriteLine("CHECK");
            return _pieces
                .Where(piece => piece.Color == player.Opponent())
                .SelectMany(piece => piece.GetAttackedSquares())
                .Any(move => move.To == kingPos);
        }

        public IEnumerable<Move> Test(Player player)
        {
            Position kingPos = FindKing(player);
            //if (kingPos is null) return false; // Safety check in case king isn't found

            return _pieces
                .Where(piece => piece.Color == player.Opponent())
                .SelectMany(piece => piece.GetAttackedSquares());
            //.Any(move => move.To == kingPos);
        }


        public ulong PositionToBit(Position pos)
        {
            int square = pos.Rank * 8 + pos.File;
            return 1UL << square;
        }

        // Build a bitboard of all pieces for a given color.
        public ulong GenerateColorBitboard(Player color)
        {
            ulong bitboard = 0;
            foreach (var piece in Pieces.Where(p => p.Color == color))
            {
                bitboard |= PositionToBit(piece.Position);
            }
            return bitboard;
        }

        // Build a bitboard for all occupied squares.
        public void GenerateOccupancy()
        {
            ulong occ = 0;
            foreach (var piece in Pieces)
            {
                occ |= PositionToBit(piece.Position);
            }
            Occupancy = occ;
        }

    }
}