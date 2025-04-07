using System.Collections.Generic;
using REPOSoundBoard.Hotkeys;
using UnityEngine;

namespace REPOSoundBoard.Config
{
    public class SoundBoardConfig
    {
        public class SoundButtonConfig
        {
            public string Path { get; set; }
            public float Volume { get; set; }
            public Hotkey Hotkey { get; set; }
        }
        
        public Hotkey StopHotkey { get; set; }
        public List<SoundButtonConfig> SoundButtons { get; set; }

        public SoundBoardConfig()
        {
            this.SoundButtons = new List<SoundButtonConfig>();
            this.StopHotkey = new Hotkey(KeyCode.H, null);
        }
    }
}