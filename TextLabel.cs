using SplashKitSDK;

namespace Chess
{
    public class TextLabel
    {
        public string Text { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        
        public TextLabel(string text, int x, int y, int width, int height)
        {
            Text = text;
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
        
        public void Draw()
        {
            SplashKit.DrawText(Text, Color.DarkBlue, X, Y);
        }
    }
} 