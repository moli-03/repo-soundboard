using System;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using REPOSoundBoard.Sound;
using REPOSoundBoard.Config;
using REPOSoundBoard.Hotkeys;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace REPOSoundBoard
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class REPOSoundBoard : BaseUnityPlugin
    {
        private const string GUID = "com.moli.repo-soundboard";
        private const string NAME = "REPOSoundBoard";
        private const string VERSION = "0.1.0";
        
        private static Harmony _harmony = new Harmony(GUID);
        public static REPOSoundBoard Instance { get; private set; }
        
        public ManualLogSource LOG => Logger;
        public AppConfig Config { get; private set; }

        public HotkeyManager HotkeyManager;
        public SoundBoard SoundBoard;
        
        private void Awake()
        {
            if (Instance != null)
            {
                return;
            }
            
            Instance = this;
            Config = AppConfig.LoadConfig();

            var go = new GameObject("REPOSoundBoardMod");
            DontDestroyOnLoad(go);
            go.hideFlags = HideFlags.HideAndDontSave;
            this.HotkeyManager = go.AddComponent<HotkeyManager>();
            this.SoundBoard = go.AddComponent<SoundBoard>();
            this.SoundBoard.LoadConfig(Config.SoundBoard);
            
            // Register patches
            _harmony.PatchAll(typeof(Patches.PlayerVoiceChatPatch));
        }
    }
}