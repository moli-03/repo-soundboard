using Newtonsoft.Json;

namespace REPOSoundBoard.Config
{
    public class ConfigSerializer
    {
        public static string SerializeConfig(AppConfig config)
        {
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            };
            
            return JsonConvert.SerializeObject(config, settings);
        }

        public static AppConfig DeserializeConfig(string json)
        {
            return JsonConvert.DeserializeObject<AppConfig>(json);
        }
    }
}