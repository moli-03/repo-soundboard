using REPOSoundBoard.Hotkeys;

namespace REPOSoundBoard.Sound
{
    public class SoundButton
    {
        public SoundClip SoundClip { get; private set; }
        public Hotkey Hotkey { get; private set; }
        public float Volume { get; private set; }

        public SoundButton(SoundClip clip, Hotkey hotkey, float volume)
        {
            this.SoundClip = clip;
            this.Hotkey = hotkey;
            this.Volume = volume;
        }
    }
}