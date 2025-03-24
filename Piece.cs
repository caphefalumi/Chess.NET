using SplashKitSDK;

namespace Chess
{
    public abstract class Piece : IDrawable
    {
        public Player Color { get; }

        public Position Position { get; set; }
        public bool HasMoved { get; set; }
        public char PieceChar { get; }
        public Bitmap PieceImage;
        public Board MyBoard { get; }
        public bool CanBeEnpassant { get; set; }

        public Piece(char pieceChar, Board board)
        {
            PieceChar = pieceChar;
            Color = char.IsUpper(pieceChar) ? Player.White : Player.Black;
            char pieceColor = (Player.White == Color) ? 'w' : 'b';
            PieceImage = new Bitmap(pieceColor.ToString() + PieceChar.ToString(), $"pieces\\{pieceColor.ToString() + PieceChar.ToString()}.png");
            HasMoved = false;
            MyBoard = board;
        }

        public HashSet<Move> GetLegalMoves()
        {
            HashSet<Move> pseudoLegalMoves = GetMoves();

            return pseudoLegalMoves.Where(move => MyBoard.GameState.MoveResolvesCheck(move, Color)).ToHashSet(); ;
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

    }
}
