using System;
using System.IO;
using BepInEx;
using REPOSoundBoard.Core.Hotkeys;
using UnityEngine;

namespace REPOSoundBoard.Config
{
    public class AppConfig
    {
        private static string _configFilePath;

        public Hotkey UiHotkey;

        public SoundBoardConfig SoundBoard;

        public AppConfig()
        {
            _configFilePath = Path.Combine(Paths.ConfigPath, "Moli.REPOSoundBoard.json");
            this.UiHotkey = new Hotkey(KeyCode.F4, null);
            this.SoundBoard = new SoundBoardConfig();
        }
        
        public static AppConfig LoadConfig()
        {
            AppConfig config = new AppConfig();

            try
            {
                string content = File.ReadAllText(_configFilePath);
                config = ConfigSerializer.DeserializeConfig(content);
                return config;
            }
            catch (Exception e)
            {
                REPOSoundBoard.Logger.LogWarning("Failed to read config file" + e.Message);
            }

            return config;
        }


        public void SaveToFile()
        {
            string json = ConfigSerializer.SerializeConfig(this);
            File.WriteAllText(_configFilePath, json);
        }
    }
}