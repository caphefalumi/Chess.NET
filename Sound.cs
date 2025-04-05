using SplashKitSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    public class Sound
    {
        private SoundEffect _soundEffect;
        public SoundType Type { get; private set; }
        private static Dictionary<string, Sound> _instances = new Dictionary<string, Sound>();

        private Sound(string fileName, SoundType type)
        {
            _soundEffect = SplashKit.LoadSoundEffect(fileName, fileName);
            Type = type;
        }

        public static Sound GetInstance(string fileName, SoundType type)
        {
            if (!_instances.ContainsKey(fileName))
            {
                _instances[fileName] = new Sound(fileName, type);
            }
            return _instances[fileName];
        }

        public void Stop()
        {
            _soundEffect.Stop();
        }

        public void Play()
        {
            _soundEffect.Play();
        }
    }
}
