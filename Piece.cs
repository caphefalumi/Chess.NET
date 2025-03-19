using SplashKitSDK;
using System.Collections.Generic;
using System.Linq;

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

        public Piece(Player color, char pieceChar)
        {
            PieceChar = pieceChar;
            char pieceColor = (Player.White == color) ? 'w' : 'b';
            PieceImage = new Bitmap(pieceColor.ToString() + PieceChar.ToString(), $"pieces\\{pieceColor.ToString() + PieceChar.ToString()}.png");
        }

        public HashSet<Move> GetLegalMoves(Board board)
        {
            HashSet<Move> pseudoLegalMoves = GetMoves(board);
            if (Type == PieceType.King)
            {

            }
            return pseudoLegalMoves.Where(move => board.GameState.MoveResolvesCheck(move)).ToHashSet();
        }

        public abstract HashSet<Move> GetMoves(Board board);

        protected bool CanMoveTo(Position pos, Board board)
        {
            return Board.IsInside(pos) && (board.IsEmpty(pos) || board.GetPieceAt(pos).Color != Color);
        }

        protected HashSet<Position> GenerateMove(Board board, Direction dir)
        {
            HashSet<Position> moves = new HashSet<Position>();

            for (Position pos = Position + dir; Board.IsInside(pos); pos += dir)
            {
                if (board.IsEmpty(pos))
                {
                    moves.Add(pos);
                }
                else
                {
                    Piece otherPiece = board.GetPieceAt(pos);
                    if (otherPiece.Color != Color)
                    {
                        moves.Add(pos);
                    }
                    break; // Stop after capturing or encountering a piece
                }
            }

            return moves;
        }

        protected HashSet<Position> GenerateMoves(Board board, Direction[] dirs)
        {
            HashSet<Position> moves = new HashSet<Position>();

            foreach (Direction dir in dirs)
            {
                moves.UnionWith(GenerateMove(board, dir));
            }

            return moves;
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
