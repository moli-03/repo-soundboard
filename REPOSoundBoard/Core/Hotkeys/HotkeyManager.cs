using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace REPOSoundBoard.Core.Hotkeys
{
    public class HotkeyManager : MonoBehaviour
    {
        private List<Hotkey> _hotkeys = new List<Hotkey>();
        
        public void RegisterHotkey(Hotkey hotkey)
        {
            _hotkeys.Add(hotkey);
        }

        public void UnregisterHotkey(Hotkey hotkey)
        {
            _hotkeys.Remove(hotkey);
        }

        public void Update()
        {
            foreach (var hotkey in this._hotkeys)
            {
                if (hotkey.IsPressed)
                {
                    HandlePressedHotkey(hotkey);
                }
                else
                {
                    HandleReleasedHotkey(hotkey);
                }
            }
        }

        private static void HandlePressedHotkey(Hotkey hotkey)
        {
            if (hotkey.Keys.Any(key => !Input.GetKey(key)))
            {
                hotkey.IsPressed = false;
            }
        }

        private static void HandleReleasedHotkey(Hotkey hotkey)
        {
            if (hotkey.Keys.All(key => Input.GetKey(key)))
            {
                hotkey.IsPressed = true;
                hotkey.Trigger();
            }
        }
    }
}