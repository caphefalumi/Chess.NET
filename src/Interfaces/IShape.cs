using SplashKitSDK;

using Chess.Core;
using Chess.Pieces;
using Chess.Moves;
namespace Chess.Interfaces
{
    public interface IShape
    {
        float X { get; set; }
        float Y { get; set; }
        Color Color { get; set; }
        void Draw();
    }
}
