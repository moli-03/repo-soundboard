using System.Collections.Generic;
using System.Linq;
using BepInEx;
using UnityEngine;

namespace REPOSoundBoard.UI.Utils
{
    public static class HotkeySelectorUtils
    {
        public static List<KeyCode> PossibleKeys { get; }

        private static readonly List<KeyCode> _disallowedKeys = new List<KeyCode>
        {
            KeyCode.Mouse0, // LMB
            KeyCode.Mouse1, // RMB
            KeyCode.LeftWindows,
            KeyCode.RightWindows,
            KeyCode.LeftApple,
            KeyCode.RightApple
        };
        
        static HotkeySelectorUtils()
        {
            PossibleKeys = UnityInput.Current.SupportedKeyCodes.Where(code => !_disallowedKeys.Contains(code)).ToList();
        }

        public static List<KeyCode> GetPressedKeys()
        {
            return PossibleKeys.Where(Input.GetKey).ToList();
        }
    }
}