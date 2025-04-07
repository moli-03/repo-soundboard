using System;
using System.IO;

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
            ConfigFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "soundboard-config.json");

            AppConfig config = new AppConfig();

            try
            {
                string content = File.ReadAllText(ConfigFilePath);
                REPOSoundBoard.Instance.LOG.LogInfo("Path: " + ConfigFilePath);
                REPOSoundBoard.Instance.LOG.LogInfo("Config: " + content);
                config = ConfigSerializer.DeserializeConfig(content);
                REPOSoundBoard.Instance.LOG.LogInfo("Loaded config: " + config);
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