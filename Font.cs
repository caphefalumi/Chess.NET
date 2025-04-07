using SplashKitSDK;

namespace Chess
{
    public static class Font
    {
        private static SplashKitSDK.Font _fontArial = SplashKit.LoadFont("Arial", "Arial.ttf");
        public static SplashKitSDK.Font Arial {get => _fontArial;}
    }
}


