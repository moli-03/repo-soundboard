using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace REPOSoundBoard.Core.Hotkeys
{
	public class Hotkey
	{
		[JsonProperty(ItemConverterType = typeof(StringEnumConverter), ObjectCreationHandling = ObjectCreationHandling.Replace)]
		public List<KeyCode> Keys { get; set; }
		
		[CanBeNull]
		[JsonIgnore]
		private Action Callback { get; set; }
		
		[JsonIgnore]
		public bool IsPressed { get; set; }

		[JsonConstructor]
		public Hotkey()
		{
			Keys = new List<KeyCode>();
		}

		public Hotkey(KeyCode key, [CanBeNull] Action callback)
		{
			Keys = new List<KeyCode> { key };
			this.Callback = callback;
		}

		public Hotkey(List<KeyCode> keys, [CanBeNull] Action callback)
		{
			Keys = new List<KeyCode>(keys);
			this.Callback = callback;
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

		public string ConcatKeys()
		{
			return string.Join(" + ", this.Keys);
		}
	}
}