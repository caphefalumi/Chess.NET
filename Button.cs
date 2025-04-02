using SplashKitSDK;

namespace Chess
{
    public class Button : IDrawable
    {
        private readonly string _text;
        private readonly Rectangle _bounds;
        private Color _normalColor;
        private Color _hoverColor;
        private Color _textColor;
        private bool _isHovered;

        public int X => (int)_bounds.X;
        public int Y => (int)_bounds.Y;
        public int Width => (int)_bounds.Width;
        public int Height => (int)_bounds.Height;

        public Button(string text, int x, int y, int width, int height)
        {
            _text = text;
            _normalColor = SplashKit.RGBColor(200, 200, 200);
            _hoverColor = SplashKit.RGBColor(180, 180, 180);
            _textColor = Color.Black;
            _bounds = new Rectangle(_normalColor, x, y, width, height);
            _isHovered = false;
        }

        public Button(string text, int x, int y, int width, int height, bool invisible)
        {
            _text = text;
            _normalColor = Color.Transparent;
            _hoverColor = Color.Transparent;
            _textColor = Color.Transparent;
            _bounds = new Rectangle(_normalColor, x, y, width, height);
            _isHovered = false;
        }

        public void Update()
        {
            _isHovered = _bounds.IsAt(SplashKit.MousePosition());
            _bounds.Color = _isHovered ? _hoverColor : _normalColor;
        }

        public bool IsClicked()
        {
            return _isHovered && SplashKit.MouseClicked(MouseButton.LeftButton);
        }

        public void Draw()
        {
            _bounds.Draw();

            Font font = SplashKit.LoadFont("Arial", "Arial.ttf");
            float textWidth = SplashKit.TextWidth(_text, font, 16);
            float textHeight = SplashKit.TextHeight(_text, font, 16);

            float textX = _bounds.X + (_bounds.Width - textWidth) / 2;
            float textY = _bounds.Y + (_bounds.Height - textHeight) / 2;

            SplashKit.DrawText(_text, _textColor, font, 16, textX, textY);
        }
    }
}