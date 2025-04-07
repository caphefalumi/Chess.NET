using SplashKitSDK;

namespace Chess
{
    public class TextLabel
    {
        public string Text { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public Color Color { get; set; }
        public int FontSize { get; set; }

        public TextLabel(string text, int x, int y)
        {
            Text = text;
            X = x;
            Y = y;
            Color = Color.DarkBlue;
            FontSize = 15;
        }
        public TextLabel(string text, int x, int y, Color color, int fontSize)
        {
            Text = text;
            X = x;
            Y = y;
            Color = color;
            FontSize = fontSize;
        }
        
        public void Draw()
        {
            SplashKit.DrawText(Text, Color, Font.Arial, FontSize, X, Y);
        }
    }
} 