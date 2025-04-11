using System.Collections.Generic;
using REPOSoundBoard.Hotkeys;
using UnityEngine;

namespace REPOSoundBoard.Config
{
    public class SoundBoardConfig
    {
        public class SoundButtonConfig
        {
            public string Name;
            public bool Enabled;
            public string Path;
            public float Volume;
            public Hotkey Hotkey;
        }

        public bool Enabled;
        public Hotkey StopHotkey { get; set; }
        public List<SoundButtonConfig> SoundButtons { get; set; }

        public SoundBoardConfig()
        {
            this.Enabled = true;
            this.SoundButtons = new List<SoundButtonConfig>();
            this.StopHotkey = new Hotkey(KeyCode.H, null);
        }
    }
}