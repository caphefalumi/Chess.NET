using SplashKitSDK;

using Chess.Core;
using Chess.Pieces;
using Chess.Moves;
using Chess.Interfaces;
using Chess.UI.Screens;
namespace Chess;
public abstract class Shape
{
    private Color _color;
    private float _x;
    private float _y;
    public Color Color
    {
        get { return _color; }
        set { _color = value; }
    }

    public float X
    {
        get { return _x; }
        set { _x = value; }
    }

    public float Y
    {
        get { return _y; }
        set { _y = value; }
    }

    public Shape(Color color)
    {
        _color = color;
        _x = 0.0f;
        _y = 0.0f;
    }

    public abstract void Draw();
    public abstract bool IsAt(Point2D pt);

}
