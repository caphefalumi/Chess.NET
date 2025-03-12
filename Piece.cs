using SplashKitSDK;

namespace Chess
{
    public abstract class Piece : IPiece
    {
        public string Name { get; }
        public string Color { get; }
        public Position Position { get; set; }
        public bool HasMoved { get; set; }
        public string Type { get; }
        public Bitmap PieceImage { get; }

        public Piece(string type, string color, Position position)
        {
            Color = color;
            Position = position;
            Type = type;
            Name = Color + Type;

            // Load image based on type and side
            string imageName = $"{Color[0].ToString().ToLower()}{Type[0].ToString().ToLower()}.png";
            PieceImage = new Bitmap(Name, $"pieces\\{imageName}");
        }

        protected virtual void AddLegalMoves((int, int)[] directions, HashSet<Position> moves, bool isSlidingPiece = false)
        {
            if (isSlidingPiece)
            {
                AddSlidingMoves(directions, moves);
            }
            else
            {
                foreach ((int dx, int dy) in directions) // Deconstruct tuple
                {
                    int newFile = Position.File + dx;
                    int newRank = Position.Rank + dy;
                    AddMoveIfLegal(newFile, newRank, moves);
                }
            }
        }

        protected void AddMoveIfLegal(int file, int rank, HashSet<Position> moves)
        {
            if (IsWithinBounds(file, rank) && Board.FindPieceAt(new Position(file, rank)) == null) // Check empty square
            {
                moves.Add(new Position(file, rank));
            }
        }

        protected void AddSlidingMoves((int, int)[] directions, HashSet<Position> moves)
        {
            foreach ((int dx, int dy) in directions)
            {
                int newFile = Position.File;
                int newRank = Position.Rank;

                while (true)
                {
                    newFile += dx;
                    newRank += dy;

                    if (!IsWithinBounds(newFile, newRank)) break;

                    IPiece pieceAtNewPos = Board.FindPieceAt(new Position(newFile, newRank));

                    if (pieceAtNewPos != null)
                    {
                        // Stop if friendly piece is encountered
                        if (pieceAtNewPos.Color == this.Color) break;

                        // Capture enemy piece
                        moves.Add(new Position(newFile, newRank));
                        break;
                    }

                    moves.Add(new Position(newFile, newRank));
                }
            }
        }

        public bool IsWithinBounds(int file, int rank)
        {
            return file >= 0 && file < 8 && rank >= 0 && rank < 8;
        }
        public bool IsWithinBounds(Position pos)
        {
            return pos.Rank >= 0 && pos.File < 8 && pos.Rank >= 0 && pos.Rank < 8;
        }

        public void Draw()
        {
            int x = Position.File * 80;
            int y = Position.Rank * 80;

            // Draw the piece image scaled to 80x80
            DrawAt(x, y);
        }


        public void DrawAt(float x, float y)
        {
            // Draw the piece centered at the given coordinates
            // You'll need to adjust based on how you're currently drawing pieces

            // If using bitmap images:
            SplashKit.DrawBitmap(PieceImage, x - PieceImage.Width / 4.3, y - PieceImage.Height / 4.3, SplashKit.OptionScaleBmp(80.0f / PieceImage.Width, 80.0f / PieceImage.Height));

            // Or if using shapes/vectors, center them at (x,y)
        }
        public abstract HashSet<Position> GetLegalMoves();
    }
}
