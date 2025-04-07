using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace REPOSoundBoard.Hotkeys
{
    
    public class Hotkey
    {
        [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
        public List<KeyCode> Keys { get; set; } = new List<KeyCode>();
        
        [CanBeNull]
        [JsonIgnore]
        private Action Callback { get; set ; }
        
        [JsonIgnore]
        public bool IsPressed { get; set; }
        
        // Empty constructor for deserialization
        public Hotkey() {  }

        public Hotkey(KeyCode key, [CanBeNull] Action callback)
        {
            Keys.Add(key);
            this.Callback = callback;
        }

        public Hotkey(List<KeyCode> keys, [CanBeNull] Action callback)
        {
            Keys = keys;
        }

        public static bool TryParse(string serializedHotkey, [CanBeNull] Action callback, out Hotkey hotkey)
        {
            var parts = serializedHotkey.Split('+');
            
            List<KeyCode> keys = new List<KeyCode>();

            foreach (var part in parts)
            {
                if (Enum.TryParse(part, out KeyCode key))
                {
                    keys.Add(key);
                }
                else
                {
                    hotkey = null;
                    return false;
                }
            }

            if (keys.Count == 0)
            {
                hotkey = null;
                return false;
            }

            hotkey = new Hotkey(keys, callback);
            return true;
        }

        public Hotkey OnPressed(Action callback)
        {
            this.Callback = callback;
            return this;
        }

        public void Trigger()
        {
            if (this.Callback == null)
            {
                return;
            }
            
            this.Callback.Invoke();
        }
    }
}