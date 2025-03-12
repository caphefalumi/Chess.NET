using SplashKitSDK;

namespace Chess
{
    public interface IShape
    {
        float X { get; set; }
        float Y { get; set; }
        Color Color { get; set; }
        void Draw();
    }
}
