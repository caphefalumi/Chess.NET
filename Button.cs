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
        private Bitmap _bitmap;
        private float _bitmapScale;

        public int X => (int)_bounds.X;
        public int Y => (int)_bounds.Y;
        public int Width => (int)_bounds.Width;
        public int Height => (int)_bounds.Height;
        public string Text => _text;

        public Button(string text, int x, int y, int width, int height)
        {
            _text = text;
            _bitmap = null;
            _normalColor = SplashKit.RGBColor(200, 200, 200);
            _hoverColor = SplashKit.RGBColor(180, 180, 180);
            _textColor = Color.Black;
            _bounds = new Rectangle(_normalColor, x, y, width, height);
            _isHovered = false;
        }

        public Button(string text, int x, int y, int width, int height, Color normalColor, Color hoverColor, Color textColor)
        {
            _text = text;
            _bitmap = null;
            _normalColor = normalColor;
            _hoverColor = hoverColor;
            _textColor = textColor;
            _bounds = new Rectangle(_normalColor, x, y, width, height);
            _isHovered = false;
        }

        public Button(Bitmap bitmap, int x, int y, int width, int height)
        {
            _text = "";
            _bitmap = bitmap;
            _normalColor = Color.White;
            _hoverColor = SplashKit.RGBColor(240, 240, 240);
            _textColor = Color.Black;
            _bounds = new Rectangle(_normalColor, x, y, width, height);
            _isHovered = false;
            
            // Calculate scale to fit the bitmap in the button while maintaining aspect ratio
            float scale = (float)width / bitmap.Width;
            _bitmapScale = scale * 0.8f;
        }

        public void Update()
        {
            _isHovered = IsAt(SplashKit.MousePosition());
            _bounds.Color = _isHovered ? _hoverColor : _normalColor;
        }

        public bool IsClicked()
        {
            return _isHovered && SplashKit.MouseClicked(MouseButton.LeftButton);
        }

        public bool IsAt(Point2D point)
        {
            return _bounds.IsAt(point);
        }

        public void Draw()
        {
            _bounds.Draw();

            if (_bitmap != null)
            {
                // Draw the bitmap centered in the button
                float bitmapWidth = _bitmap.Width * _bitmapScale;
                float bitmapHeight = _bitmap.Height * _bitmapScale;
                
                float bitmapX = _bounds.X - 35 + (_bounds.Width - bitmapWidth) / 2;
                float bitmapY = _bounds.Y - 35 + (_bounds.Height - bitmapHeight) / 2;
                
                SplashKit.DrawBitmap(
                    _bitmap, 
                    bitmapX, 
                    bitmapY, 
                    SplashKit.OptionScaleBmp(_bitmapScale, _bitmapScale)
                );
            }
            else if (!string.IsNullOrEmpty(_text))
            {
                // Draw the text centered in the button
                float textWidth = SplashKit.TextWidth(_text, Font.Arial, 16);
                float textHeight = SplashKit.TextHeight(_text, Font.Arial, 16);

                float textX = _bounds.X + (_bounds.Width - textWidth) / 2;
                float textY = _bounds.Y + (_bounds.Height - textHeight) / 2;

                SplashKit.DrawText(_text, _textColor, Font.Arial, 16, textX, textY);
            }
        }
    }
}