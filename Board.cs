using SplashKitSDK;
namespace Chess
{
    public class Board
    {
        public HashSet<Piece> pieces;
        public HashSet<IShape> shapes;
        private static Board _instance;
        private static BoardDrawer _boardDrawer;
        
        public Piece this[int rank, int file]
        {
            get => pieces.FirstOrDefault(p => p.Position.Rank == rank && p.Position.File == file);
            set
            {
                Piece existingPiece = pieces.FirstOrDefault(p => p.Position.Rank == rank && p.Position.File == file);
                if (existingPiece != null)
                    pieces.Remove(existingPiece); // Remove old piece at position

                if (value != null)
                    pieces.Add(value); // Add new piece
            }
        }
        public Piece this[Position pos]
        {
            get => pieces.FirstOrDefault(p => p.Position.Rank == pos.Rank && p.Position.File == pos.File);
            set
            {
                Piece existingPiece = pieces.FirstOrDefault(p => p.Position.Rank == pos.Rank && p.Position.File == pos.File);
                if (existingPiece != null)
                    pieces.Remove(existingPiece); // Remove old piece at position

                if (value != null)
                    pieces.Add(value); // Add new piece
            }
        }

        private Board(int squareSize, int startX, int startY, Color lightColor, Color darkColor)
        {
            pieces = PieceFactory.CreatePieces();
            shapes = new HashSet<IShape>();
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
            _boardDrawer.Draw(pieces);
            _boardDrawer.Draw(shapes);
        }

        public static bool IsInside(Position pos)
        {
            return pos.File >= 0 && pos.File < 8 && pos.Rank >= 0 && pos.Rank < 8;
        }
        public bool IsEmpty(Position pos)
        {
            return this[pos] == null;
        }
    }
}