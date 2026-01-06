using SplashKitSDK;

using Chess.Core;
using Chess.Moves;
namespace Chess.Pieces
{
    public abstract class Piece : IDrawable
    {
        private readonly Player _color;
        private readonly char _pieceChar;
        private readonly PieceType _type;
        private readonly Bitmap _pieceImage;
        private readonly Board _myBoard;

        public Position Position { get; set; }
        public bool HasMoved { get; set; }
        public bool IsSelected { get; set; }

        public Player Color => _color;
        public char PieceChar => _pieceChar;
        public PieceType Type => _type;
        public Bitmap PieceImage => _pieceImage;
        public Board MyBoard => _myBoard;

        public Piece(char pieceChar, Board board)
        {
            _pieceChar = pieceChar;
            _color = char.IsUpper(pieceChar) ? Player.White : Player.Black;
            _type = PieceFactory.GetPieceType(_pieceChar);
            _pieceImage = GetPieceBitmap(_type, _color);
            _myBoard = board;

            HasMoved = false;
            IsSelected = false;
        }

        public HashSet<Move> GetLegalMoves()
        {
            HashSet<Move> pseudoLegalMoves = GetMoves();
            return pseudoLegalMoves
                .Where(move => _myBoard.MatchState.MoveResolvesCheck(move, _color))
                .ToHashSet();
        }

        public abstract HashSet<Move> GetMoves();

        protected bool CanMoveTo(Position pos)
        {
            return Board.IsInside(pos) &&
                   (_myBoard.IsEmpty(pos) || _myBoard.GetPieceAt(pos).Color != _color);
        }

        public virtual IEnumerable<Move> GetAttackedSquares()
        {
            return GetMoves();
        }

        public void Draw()
        {
            int x = Position.File * 80 - 35;
            int y = Position.Rank * 80 - 35;
            SplashKit.DrawBitmap(
                _pieceImage,
                x,
                y,
                SplashKit.OptionScaleBmp(80.0f / _pieceImage.Width, 80.0f / _pieceImage.Height)
            );
        }

        public static Bitmap GetPieceBitmap(PieceType pieceType, Player color)
        {
            char pieceChar = PieceFactory.GetPieceChar(pieceType, color);
            char pieceColor = color == Player.White ? 'w' : 'b';
            string bitmapName = pieceColor + pieceChar.ToString();
            return SplashKit.LoadBitmap(bitmapName, $"Resources/Pieces/{bitmapName}.png");
        }
    }
}
