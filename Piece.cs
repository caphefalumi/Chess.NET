using SplashKitSDK;

namespace Chess
{
    public abstract class Piece : IDrawable
    {
        public Player Color { get; }
        public Position Position { get; set; }
        public bool HasMoved { get; set; }
        public char PieceChar { get; }
        public PieceType Type { get; }
        public Bitmap PieceImage;
        public Board MyBoard { get; }
        public bool IsSelected { get; set; }

        public Piece(char pieceChar, Board board)
        {
            PieceChar = pieceChar;
            Color = char.IsUpper(pieceChar) ? Player.White : Player.Black;
            Type = PieceFactory.GetPieceType(PieceChar);
            char pieceColor = (Player.White == Color) ? 'w' : 'b';
            PieceImage = new Bitmap(pieceColor.ToString() + PieceChar.ToString(), $"Resources\\Pieces\\{pieceColor.ToString() + PieceChar.ToString()}.png");
            HasMoved = false;
            MyBoard = board;
            IsSelected = false;
        }

        public HashSet<Move> GetLegalMoves()
        {
            HashSet<Move> pseudoLegalMoves = GetMoves();

            return pseudoLegalMoves.Where(move => MyBoard.MatchState.MoveResolvesCheck(move, Color)).ToHashSet(); ;
        }

        public abstract HashSet<Move> GetMoves();

        protected bool CanMoveTo(Position pos)
        {
            return Board.IsInside(pos) && (MyBoard.IsEmpty(pos) || MyBoard.GetPieceAt(pos).Color != Color);
        }

        public virtual IEnumerable<Move> GetAttackedSquares()
        {
            return GetMoves();
        }
        public void Draw()
        {
            int x = Position.File * 80;
            int y = Position.Rank * 80;

            DrawAt(x - 35, y - 35);
        }

        public void DrawAt(float x, float y)
        {
            SplashKit.DrawBitmap(PieceImage, x, y, SplashKit.OptionScaleBmp(80.0f / PieceImage.Width, 80.0f / PieceImage.Height));
        }
                // Helper method to get piece bitmap without creating new bitmap objects
        public static Bitmap GetPieceBitmap(PieceType pieceType, Player color)
        {
            // Get the piece character from the PieceFactory
            char pieceChar = PieceFactory.GetPieceChar(pieceType, color);
            
            // Get the bitmap filename directly without creating a temporary piece
            char pieceColor = (color == Player.White) ? 'w' : 'b';
            string bitmapName = pieceColor.ToString() + pieceChar.ToString();
            
            // Try to load the bitmap using SplashKit's bitmap management
            // This will reuse existing bitmaps rather than creating new ones
            return SplashKit.LoadBitmap(bitmapName, $"Resources\\Pieces\\{bitmapName}.png");
        }
    }
}
