using SplashKitSDK;

namespace Chess
{
    public class Button
    {
        private string _text;
        private Rectangle _rect;
        private Color _normalColor;
        private Color _hoverColor;
        private Color _textColor;
        private bool _isHovered;

        public Button(string text, int x, int y, int width, int height)
        {
            _text = text;
            _normalColor = SplashKit.RGBColor(200, 200, 200);
            _hoverColor = SplashKit.RGBColor(180, 180, 180);
            _textColor = Color.Black;
            _rect = new Rectangle(_normalColor, x, y, width, height);
            _isHovered = false;
        }

        // Implementation as in the previous response
    }
}