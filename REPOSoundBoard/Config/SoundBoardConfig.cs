using System.Collections.Generic;
using REPOSoundBoard.Core.Hotkeys;
using UnityEngine;

namespace REPOSoundBoard.Config
{
    public class SoundBoardConfig
    {
        public bool Enabled = true;
        public Hotkey StopHotkey = new Hotkey(KeyCode.H, null);
        public List<SoundButtonConfig> SoundButtons = new List<SoundButtonConfig>();
    }
}