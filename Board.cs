using SplashKitSDK;
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
        }
        public int SquareSize
        {
            get => _squareSize;
        }

        public Color LightColor
        {
            get => _lightColor;
        }

        public Color DarkColor
        {
            get => _darkColor;
        }

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
            _pieces = PieceFactory.CreatePieces();
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
            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    Position pos = new Position(rank ,file);
                    Piece piece = GetPieceAt(pos);

                    if (piece is not null && piece.Color == player)
                    {
                        yield return pos;
                    }
                }
            }
        }



        public IEnumerable<Position> PiecePositionsFor(Player player)
        {
            return PiecePositions(player);
        }
        private Position FindKing(Player player)
        {
            foreach (Piece piece in Pieces)
            {
                if (piece.Type == PieceType.King && piece.Color == player)
                {
                    return piece.Position;
                }
            }
            return null;
        }

        public bool IsInCheck(Player player)
        {
            Position kingPos = FindKing(player.Opponent());  // Find the player's king

            return PiecePositionsFor(player.Opponent()).Any(pos =>
            {
                Piece piece = GetPieceAt(pos);
                return piece.GetMoves(this).Any(move => move.To == kingPos);
            });
        }

        public Board Copy()
        {
            Board copy = (Board)MemberwiseClone();
            return copy;
        }
    }
}