using SplashKitSDK;
using System.Xml.Linq;

namespace Chess
{
    public abstract class Piece : IDrawable
    {
        public abstract PieceType Type { get; }
        public abstract Player Color { get; }
        public abstract Piece Copy();

        public Position Position { get; set; }
        public bool HasMoved { get; set; } = false;
        public char PieceChar { get; }
        public Bitmap PieceImage;

        public Piece(char pieceChar)
        {
            PieceChar = pieceChar;
            char pieceColor = char.IsUpper(pieceChar) ? 'w' : 'b';
            PieceImage = new Bitmap(pieceColor.ToString() + PieceChar.ToString() , $"pieces\\{pieceColor.ToString() + PieceChar.ToString()}.png");
        }

        public abstract IEnumerable<Move> GetMoves(Board board);
        protected bool CanMoveTo(Position pos, Board board)
        {
            return Board.IsInside(pos) && (board.IsEmpty(pos) || board.GetPieceAt(pos).Color != Color);
        }

        protected IEnumerable<Position> GenerateMove(Board board, Direction dir)
        {
            for (Position pos = Position + dir; Board.IsInside(pos); pos += dir)
            {
                if (board.IsEmpty(pos))
                {
                    yield return pos;
                }
                else
                {
                    Piece otherPiece = board.GetPieceAt(pos);
                    if (otherPiece.Color != Color)
                    {
                        yield return pos;
                    }
                    yield break;
                }
            }
        }

        protected IEnumerable<Position> GenerateMoves(Board board, Direction[] dirs)
        {
            return dirs.SelectMany(dir => GenerateMove(board, dir));
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

        public virtual bool CanCaptureOpponentKing(Board board)
        {
            return GetMoves(board).Any(move =>
            {
                Piece piece = board.GetPieceAt(move.To);
                return piece != null && piece.Type == PieceType.King && piece.Color != Color;
            });
        }
    }
} 