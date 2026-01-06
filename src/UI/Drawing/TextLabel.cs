using SplashKitSDK;

using Chess.Core;
using Chess.Pieces;
using Chess.Moves;
using Chess.Interfaces;
using Chess.UI.Screens;
namespace Chess.UI.Drawing
{
    public class TextLabel
    {
        private string _text;
        private int _x;
        private int _y;
        private Color _color;
        private int _fontSize;

        public TextLabel(string text, int x, int y)
        {
            _text = text;
            _x = x;
            _y = y;
            _color = Color.DarkBlue;
            _fontSize = 15;
        }

        public TextLabel(string text, int x, int y, Color color, int fontSize)
        {
            _text = text;
            _x = x;
            _y = y;
            _color = color;
            _fontSize = fontSize;
        }

        public string Text
        {
            get => _text;
            set => _text = value;
        }

        public int X
        {
            get => _x;
            set => _x = value;
        }

        public int Y
        {
            get => _y;
            set => _y = value;
        }

        public Color Color
        {
            get => _color;
            set => _color = value;
        }

        public int FontSize
        {
            get => _fontSize;
            set => _fontSize = value;
        }

        public void Draw()
        {
            SplashKit.DrawText(_text, _color, Font.Arial, _fontSize, _x, _y);
        }
    }
}
