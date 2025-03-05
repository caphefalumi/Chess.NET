using Chess;
using SplashKitSDK;

namespace Chess
{
    public class MyCircle : Shape
    {
        int _radius;
        public MyCircle()
        {
            Color = Color.Yellow;
            _radius = 50;
        }

        public MyCircle(Color color, float x, float y, int radius)
        {
            Color = color;
            X = x;
            Y = y;
            _radius = radius;
        }

        public int Radius
        {
            get { return _radius; }
            set { _radius = value; }
        }

        public override void Draw()
        {
            SplashKit.FillCircle(Color, X, Y, _radius);
        }

    }
}
