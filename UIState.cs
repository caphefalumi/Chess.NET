using SplashKitSDK;

namespace Chess
{
    public abstract class ScreenState
    {
        public abstract void HandleInput();
        public abstract void Update();
        public abstract void Render();
        public abstract string GetStateName();
    }
}
