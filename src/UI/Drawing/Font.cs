using SplashKitSDK;

using Chess.Core;
using Chess.Pieces;
using Chess.Moves;
using Chess.Interfaces;
using Chess.UI.Screens;
namespace Chess.UI.Drawing
{
    public static class Font
    {
        private static SplashKitSDK.Font _fontArial = SplashKit.LoadFont("Arial", "Arial.ttf");
        public static SplashKitSDK.Font Arial {get => _fontArial;}
    }
}


