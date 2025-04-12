using REPOSoundBoard.Hotkeys;

namespace REPOSoundBoard.Config
{
    public class SoundButtonConfig
    {
        public string Name;
        public bool Enabled = true;
        public string Path;
        public float Volume = 0.75f;
        public Hotkey Hotkey;
    }
}