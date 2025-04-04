using System.Collections.Generic;
using SplashKitSDK;

namespace Chess
{
    public enum SoundType
    {
        Capture,
        Castle,
        GameEnd,
        GameStart,
        Illegal,
        MoveCheck,
        MoveSelf,
        Promote
    }

    public static class Sounds
    {
        private static readonly Dictionary<SoundType, Sound> _sounds = new()
        {
            { SoundType.Capture, Sound.GetInstance("Resources/Sounds/capture.mp3") },
            { SoundType.Castle, Sound.GetInstance("Resources/Sounds/castle.mp3") },
            { SoundType.GameEnd, Sound.GetInstance("Resources/Sounds/game_end.mp3") },
            { SoundType.GameStart, Sound.GetInstance("Resources/Sounds/game_start.mp3") },
            { SoundType.Illegal, Sound.GetInstance("Resources/Sounds/illegal.mp3") },
            { SoundType.MoveCheck, Sound.GetInstance("Resources/Sounds/move_check.mp3") },
            { SoundType.MoveSelf, Sound.GetInstance("Resources/Sounds/move_self.mp3") },
            { SoundType.Promote, Sound.GetInstance("Resources/Sounds/promote.mp3") }
        };

        public static Sound Capture => _sounds[SoundType.Capture];
        public static Sound Castle => _sounds[SoundType.Castle];
        public static Sound GameEnd => _sounds[SoundType.GameEnd];
        public static Sound GameStart => _sounds[SoundType.GameStart];
        public static Sound Illegal => _sounds[SoundType.Illegal];
        public static Sound MoveCheck => _sounds[SoundType.MoveCheck];
        public static Sound MoveSelf => _sounds[SoundType.MoveSelf];
        public static Sound Promote => _sounds[SoundType.Promote];
    }
}
