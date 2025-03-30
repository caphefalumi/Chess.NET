using Chess;
using SplashKitSDK;

namespace Chess
{
    public class Circle : Shape, IShape
    {
        int _radius;

        public Circle(Color color, float x, float y, int radius) : base(color)
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
        public override bool IsAt(Point2D pt)
        {
            SplashKitSDK.Circle circle = SplashKit.CircleAt(X, Y, _radius);
            return SplashKit.PointInCircle(pt, circle);
        }

        public override void Draw()
        {
            SplashKit.FillCircle(Color, X, Y, _radius);
        }

    }
}
