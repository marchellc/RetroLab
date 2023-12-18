using RetroLab.Server.Utilities;

namespace RetroLab.Server.Core
{
    public static class Paths
    {
        public static string Main;
        public static string Net;

        public static void Load()
        {
            Main = Directory.GetCurrentDirectory();
            Net = $"{Main}/Network";

            CheckDir(Net);
        }

        public static T GetJson<T>(string main, string file, T defaultValue)
        {
            var path = $"{main}/{file}";

            Program.Log.Trace($"Loading JSON ({typeof(T).FullName}) from file '{path}'");

            CheckDir(main);
           
            if (!File.Exists(path))
            {
                Program.Log.Trace($"File does not exist, writing default ..");

                File.WriteAllText(path, JsonHelper.Serialize(defaultValue));
                return defaultValue;
            }

            Program.Log.Trace($"File exists, attempting to deserialize ..");
            return JsonHelper.Deserialize<T>(File.ReadAllText(path));
        }

        public static void Write(string main, string file, object value)
            => File.WriteAllText($"{main}/{file}", JsonHelper.Serialize(value));

        public static void CheckDir(string dir)
        {
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }

        public static void CheckFile(string file)
        {
            if (!File.Exists(file))
                File.Create(file).Close();
        }
    }
}
