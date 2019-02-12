using System;

namespace Topaz.Engine
{
    [Serializable]
    public class Settings
    {
        public VideoSettings Video { get; set; }
        public AudioSettings Audio { get; set; }

        public Settings()
        {
            Video = new VideoSettings();
            Audio = new AudioSettings();
        }

        [Serializable]
        public class VideoSettings
        {
            public bool Fullscreen { get; set; }
            public bool Vsync { get; set; }
            public int WindowedScreenWidth { get; set; }
            public int WindowedScreenHeight { get; set; }

            public VideoSettings()
            {
                Fullscreen = false;
                Vsync = true;
                WindowedScreenWidth = 1280;
                WindowedScreenHeight = 800;
            }
        }

        [Serializable]
        public class AudioSettings
        {
            public bool MusicMute { get; set; }
            public bool SoundsMute { get; set; }
            public int MusicVolume { get; set; }
            public int SoundsVolume { get; set; }

            public AudioSettings()
            {
                MusicMute = true;
                SoundsMute = false;
                MusicVolume = 100;
                SoundsVolume = 100;
            }
        }
    }
}
