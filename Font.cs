using SplashKitSDK;

namespace Chess
{
    public static class Font
    {
        private static SplashKitSDK.Font _font = SplashKit.LoadFont("Arial", "Arial.ttf");
        public static SplashKitSDK.Font Get {get => _font;}
    }
}


