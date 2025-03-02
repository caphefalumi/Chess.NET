using SplashKitSDK;

namespace Chess
{
    public abstract class Piece
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

            Name = Color + Type;

            // Load image based on type and side
            string imageName = $"{Color[0].ToString().ToLower()}{Type[0].ToString().ToLower()}.png";
            Console.WriteLine(imageName);

            PieceImage = SplashKit.LoadBitmap(Name, $"pieces\\{imageName}");
        }

        public void Draw()
        {
            int x = Position.File * 80 - 40;
            int y = Position.Rank * 80 - 40;

            // Scale the image to 80x80 while drawing
            SplashKit.DrawBitmap(PieceImage, x, y, SplashKit.OptionScaleBmp(80.0f / PieceImage.Width, 80.0f / PieceImage.Height));
        }
    }
}
