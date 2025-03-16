using SplashKitSDK;
using System.Xml.Linq;

namespace Chess
{
    public abstract class Piece
    {
        public abstract PieceType Type { get; }
        public abstract Player Color { get; }
        public Position Position { get; set; }
        public bool HasMoved { get; set; } = false;
        public char PieceChar { get; }
        public Bitmap PieceImage;
        public abstract Piece Copy();

        public Piece(char pieceChar)
        {
            PieceChar = pieceChar;
            char pieceColor = char.IsUpper(pieceChar) ? 'w' : 'b';
            PieceImage = new Bitmap(pieceColor.ToString() + PieceChar.ToString() , $"pieces\\{pieceColor.ToString() + PieceChar.ToString()}.png");
            Console.WriteLine($"pieces\\{PieceChar + pieceColor}.png");
        }

        public abstract IEnumerable<Move> GetMoves(Position from, Board board);
        protected bool CanMoveTo(Position pos, Board board)
        {
            return Board.IsInside(pos) && (board.IsEmpty(pos) || board[pos].Color != Color);
        }

        protected IEnumerable<Position> GenerateMove(Position from, Board board, Direction dir)
        {
            for (Position pos = from + dir; Board.IsInside(pos); pos += dir)
            {
                if (board.IsEmpty(pos))
                {
                    yield return pos;
                }
                else
                {
                    Piece otherPiece = board[pos];
                    if (otherPiece.Color != Color)
                    {
                        yield return pos;
                    }
                    yield break;
                }
            }
        }

        protected IEnumerable<Position> GenerateMoves(Position from, Board board, Direction[] dirs)
        {
            return dirs.SelectMany(dir => GenerateMove(from, board, dir));
        }
        public void Draw()
        {
            int x = Position.File * 80;
            int y = Position.Rank * 80;

            DrawAt(x - 35, y - 35);
        }

        // Implement the DrawAt method required by IPiece interface
        public void DrawAt(float x, float y)
        {
            // Draw the piece image scaled to 80x80 at the specified position
            SplashKit.DrawBitmap(PieceImage, x, y, SplashKit.OptionScaleBmp(80.0f / PieceImage.Width, 80.0f / PieceImage.Height));
        }

    }
} 