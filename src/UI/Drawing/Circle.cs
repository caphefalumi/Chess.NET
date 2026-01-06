using SplashKitSDK;

using Chess.Core;
using Chess.Pieces;
using Chess.Moves;
using Chess.Interfaces;
using Chess.UI.Screens;
namespace Chess.UI.Drawing
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
