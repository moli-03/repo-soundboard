using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using REPOSoundBoard.Core;
using REPOSoundBoard.Config;
using REPOSoundBoard.Core.Hotkeys;
using REPOSoundBoard.UI;
using UnityEngine;

namespace REPOSoundBoard
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class REPOSoundBoard : BaseUnityPlugin
    {
        public const string GUID = "com.moli.repo-soundboard";
        public const string NAME = "REPOSoundBoard";
        public const string VERSION = "0.2.0";
        
        private static readonly Harmony _harmony = new Harmony(GUID);
        public static REPOSoundBoard Instance { get; private set; }
        
        public new static ManualLogSource Logger;
        public new AppConfig Config { get; private set; }

        public HotkeyManager HotkeyManager;
        public SoundBoard SoundBoard;
        public SoundBoardUI UI;
        
        private void Awake()
        {
            if (Instance != null)
            {
                return;
            }
            
            Instance = this;
            REPOSoundBoard.Logger = base.Logger;
            Config = AppConfig.LoadConfig();

            var go = new GameObject("REPOSoundBoardMod");
            go.hideFlags = HideFlags.HideAndDontSave;
            DontDestroyOnLoad(go);
            
            this.HotkeyManager = go.AddComponent<HotkeyManager>();
            
            this.SoundBoard = go.AddComponent<SoundBoard>();
            this.SoundBoard.LoadConfig(Config.SoundBoard);
            
            this.UI = go.AddComponent<SoundBoardUI>();
            Config.UiHotkey.OnPressed(() => UI.Visible = !UI.Visible);
            this.HotkeyManager.RegisterHotkey(Config.UiHotkey);
            
            // Register patches
            _harmony.PatchAll(typeof(Patches.ChatManagerPatch));
            _harmony.PatchAll(typeof(Patches.PlayerVoiceChatPatch));
            _harmony.PatchAll(typeof(Patches.MenuCursorPatch));
        }

        public void SaveConfig()
        {
            // Update the config
            var soundBoardConfig = new SoundBoardConfig();
            soundBoardConfig.Enabled = SoundBoard.Instance.Enabled;
            soundBoardConfig.StopHotkey = SoundBoard.Instance.StopHotkey;

            foreach (var soundButton in SoundBoard.Instance.SoundButtons)
            {
                soundBoardConfig.SoundButtons.Add(soundButton.CreateConfig());
            }

            Config.SoundBoard = soundBoardConfig;
            Config.SaveToFile();
        }
    }
}