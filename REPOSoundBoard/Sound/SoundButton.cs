using REPOSoundBoard.Hotkeys;

namespace REPOSoundBoard.Sound
{
    public class SoundButton
    {
        public MediaClip Clip { get; private set; }
        public Hotkey Hotkey { get; private set; }
        public float Volume { get; private set; }

        public SoundButton(MediaClip clip, Hotkey hotkey, float volume)
        {
            this.Clip = clip;
            this.Hotkey = hotkey;
            this.Volume = volume;
        }
    }
}