using SplashKitSDK;

namespace Chess
{
    public class Button : IDrawable
    {
        private string _text;
        private Rectangle _rect;
        private Color _normalColor;
        private Color _hoverColor;
        private Color _textColor;
        private bool _isHovered;

        public Rectangle Rect => _rect;

        public Button(string text, int x, int y, int width, int height)
        {
            _text = text;
            _normalColor = SplashKit.RGBColor(200, 200, 200);
            _hoverColor = SplashKit.RGBColor(180, 180, 180);
            _textColor = Color.Black;
            _rect = new Rectangle(_normalColor, x, y, width, height);
            _isHovered = false;
        }
        public Button(int x, int y, int width, int height)
        {
            _text = "";
            _normalColor = Color.Transparent;
            _hoverColor = Color.Transparent;
            _textColor = Color.Transparent;
            _rect = new Rectangle(_normalColor, x, y, width, height);
            _isHovered = false;
        }
        public void Update()
        {
            _isHovered = _rect.IsAt(SplashKit.MousePosition());
            _rect.Color = _isHovered ? _hoverColor : _normalColor;
        }

        public bool IsClicked()
        {
            return _isHovered && SplashKit.MouseClicked(MouseButton.LeftButton);
        }

        public void Draw()
        {
            _rect.Draw();

            Font font = SplashKit.LoadFont("Arial", "Arial.ttf");

            int textWidth = SplashKit.TextWidth(_text, font, 16);
            int textHeight = SplashKit.TextHeight(_text, font, 16);

            float textX = _rect.X + (_rect.Width - textWidth) / 2;
            float textY = _rect.Y + (_rect.Height - textHeight) / 2;

            SplashKit.DrawText(_text, _textColor, font, 16, textX, textY);
        }
    }
}