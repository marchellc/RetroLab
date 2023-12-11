using Newtonsoft.Json;

using System.IO;

namespace RetroLab
{
    public class Config
    {
        public string Ip { get; set; } = "127.0.0.1";

        public int Port { get; set; } = 8888;

#region STATIC

        public static Config Instance;

        public static void Load()
        {
            var path = $"{Directory.GetCurrentDirectory()}/config.json";

            if (!File.Exists(path))
            {
                Instance = new Config();
                Save();
                return;
            }

            Instance = JsonConvert.DeserializeObject<Config>(File.ReadAllText(path));
        }

        public static void Save()
        {
            File.WriteAllText($"{Directory.GetCurrentDirectory()}/config.json", JsonConvert.SerializeObject(Instance));
        }
    }
#endregion
}