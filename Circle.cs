using Chess;
using SplashKitSDK;

namespace Chess
{
    public class Circle : Shape, IShape
    {
        int _radius;
        public Circle()
        {
            Color = Color.Yellow;
            _radius = 50;
        }

        public Circle(Color color, float x, float y, int radius)
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
