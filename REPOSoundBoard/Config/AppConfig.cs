using System;
using System.IO;
using BepInEx;

namespace REPOSoundBoard.Config
{
    public class AppConfig
    {
        private static string ConfigFilePath;
        
        public SoundBoardConfig SoundBoard { get; set; }

        public AppConfig()
        {
            this.SoundBoard = new SoundBoardConfig();
        }
        
        public static AppConfig LoadConfig()
        {
            ConfigFilePath = Path.Combine(Paths.ConfigPath, "Moli.REPOSoundBoard.json");

            AppConfig config = new AppConfig();

            try
            {
                string content = File.ReadAllText(ConfigFilePath);
                config = ConfigSerializer.DeserializeConfig(content);
                return config;
            }
            catch (Exception e)
            {
                REPOSoundBoard.Instance.LOG.LogWarning("Failed to read config file" + e.Message);
            }

            return config;
        }


        public void SaveToFile()
        {
            string json = ConfigSerializer.SerializeConfig(this);
            File.WriteAllText(ConfigFilePath, json);
        }
    }
}