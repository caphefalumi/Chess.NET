using SplashKitSDK;
using Chess.Core;
using Chess.UI.Drawing;
using Chess.UI.Screens;

namespace Chess.UI.States
{
    public abstract class ScreenState
    {
        public abstract void HandleInput();
        public abstract void Update();
        public abstract void Render();
    }
}
