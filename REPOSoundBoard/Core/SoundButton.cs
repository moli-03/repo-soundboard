using System.Collections;
using REPOSoundBoard.Config;
using REPOSoundBoard.Core.Media;
using REPOSoundBoard.Core.Hotkeys;

namespace REPOSoundBoard.Core
{
    public class SoundButton
    {
        public string Name;
        public bool Enabled;
        public float Volume;
        public Hotkey Hotkey;
        public MediaClip Clip;
        
        public static SoundButton FromConfig(SoundButtonConfig config)
        {
            var sb = new SoundButton();
            sb.Name = config.Name ?? "";
            sb.Enabled = config.Enabled;
            sb.Volume = config.Volume;
            sb.Hotkey = config.Hotkey;
            sb.Clip = new MediaClip(config.Path);

            return sb;
        }
        
        public SoundButtonConfig CreateConfig()
        {
            var config = new SoundButtonConfig();
            config.Name = this.Name;
            config.Volume = this.Volume;
            config.Hotkey = this.Hotkey;
            config.Path = this.Clip.OriginalPath;
            config.Enabled = this.Enabled;
            return config;
        }
        
        public IEnumerator LoadClip()
        {
            return this.Clip.Load();
        }
    }
}