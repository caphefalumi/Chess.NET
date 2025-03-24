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

        public Sound(string fileName)
        {
            _soundEffect = SplashKit.LoadSoundEffect(fileName, fileName);
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
