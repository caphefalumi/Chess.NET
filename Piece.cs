using SplashKitSDK;

namespace Chess
{
    public abstract class Piece : IPiece
    {
        public string Name { get; }
        public string Color { get; }
        public Position Position { get; set; }
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

        public void Draw()
        {
            int x = Position.File * 80 - 35;
            int y = Position.Rank * 80 - 35;

            // Draw the piece image scaled to 80x80
            SplashKit.DrawBitmap(PieceImage, x, y, SplashKit.OptionScaleBmp(80.0f / PieceImage.Width, 80.0f / PieceImage.Height));
        }


        //public abstract bool IsValidMove(Position newPosition);
    }
}
