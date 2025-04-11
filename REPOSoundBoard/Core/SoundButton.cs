using REPOSoundBoard.Core.Media;
using REPOSoundBoard.Hotkeys;

namespace REPOSoundBoard.Core
{
    public class SoundButton
    {
        public string Name;
        public bool Enabled;
        public float Volume;
        public Hotkey Hotkey;
        public MediaClip Clip;

        public SoundButton(string name, float volume, Hotkey hotkey, MediaClip clip)
        {
            Name = name;
            Hotkey = hotkey;
            Clip = clip;
            Volume = volume;
        }
    }
}