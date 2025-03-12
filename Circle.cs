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
        public void Draw(int thickness)
        {
            Color backgroundColor;
            // Draw a filled circle as the "border"
            SplashKit.FillCircle(Color, X, Y, _radius);
            if (((int)X + (int)Y) % 2 != 0)
            {
                backgroundColor = SplashKit.RGBColor(255, 206, 158);
            }
            else
            {
                backgroundColor = SplashKit.RGBColor(209, 139, 71);
            }
            // Draw a smaller filled circle with background color to create the effect of thickness
            SplashKit.FillCircle(backgroundColor, X, Y, _radius - thickness);
        }

    }
}
