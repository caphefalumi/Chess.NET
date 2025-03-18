using System.IO;
using SplashKitSDK;

namespace Chess
{

    public class Rectangle : Shape, IShape
    {
        int _width;
        int _height;

        public Rectangle(Color color, float x, float y, int width, int height) : base(color)
        {
            Color = color;
            X = x;
            Y = y;
            _width = width;
            _height = height;

        }

        public Rectangle(Color color, float x, float y, int squareSize) : base(color)
        {
            Color = color;
            X = x;
            Y = y;
            _width = squareSize;
            _height = squareSize;

        }
        public override void Draw()
        {
            SplashKit.FillRectangle(Color, X, Y, _width, _height);
        }
         
        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }

        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }
    }
}
