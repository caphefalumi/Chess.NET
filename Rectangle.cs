using System.IO;
using SplashKitSDK;

namespace Chess
{

    public class MyRectangle : Shape
    {
        int _width;
        int _height;

        public MyRectangle(Color color, float x, float y, int width, int height) : base(color)
        {
            Color = color;
            X = x;
            Y = y;
            _width = width;
            _height = height;

        }

        public MyRectangle() : this(Color.Red, 0, 0, 100 + 2, 100 + 2)
        {
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
