using Common.IO.Collections;

namespace RetroLab.Server.Core
{
    public static class Args
    {
        private static LockedDictionary<string, string> keys = new LockedDictionary<string, string>();
        private static LockedList<string> switches = new LockedList<string>();

        public static IReadOnlyDictionary<string, string> Keys { get; private set; }
        public static IReadOnlyList<string> Switches { get; private set; }

        public static bool TryParse(string[] args)
        {
            if (args is null || args.Length <= 0)
                return true;

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith("--"))
                {
                    if (!args[i].Contains("="))
                    {
                        Program.Log?.Warn($"Failed to parse argument '{args[i]}' at {i}");
                        return false;
                    }

                    var split = args[i].Split('=', 2, StringSplitOptions.RemoveEmptyEntries);
                    
                    if (split.Length != 2)
                    {
                        Program.Log?.Warn($"Failed to parse argument '{args[i]}' at {i}");
                        return false;
                    }

                    var key = split[0].Replace("--", "").Trim();
                    var value = split[0].Trim();

                    if (keys.ContainsKey(key))
                    {
                        Program.Log?.Warn($"Failed to parse argument '{args[i]}' at {i}: this argument already exists");
                        return false;
                    }

                    keys[key] = value;

                    Program.Log?.Trace($"Loaded argument: {key} ({value})");
                }
                else if (args[i].StartsWith("-"))
                {
                    var switchName = args[i].Trim('-');

                    if (switches.Contains(switchName))
                    {
                        Program.Log?.Warn($"Failed to parse argument '{args[i]}' at {i}: this switch already exists");
                        return false;
                    }

                    switches.Add(switchName);

                    Program.Log?.Trace($"Loaded switch: {switchName}");
                }
                else
                {
                    Program.Log?.Warn($"Failed to parse argument '{args[i]}' at {i}");
                    return false;
                }
            }

            Keys = keys.AsReadOnly();
            Switches = switches.AsReadOnly();

            keys.Clear();
            keys = null;

            switches.Clear();
            switches = null;

            Program.Log?.Info($"Parsed {Keys.Count} key(s) and {Switches.Count} switch(es) from {args.Length} startup argument(s).");

            return true;
        }

        public static bool HasSwitch(string switchName)
            => Switches.Contains(switchName);

        public static string GetValue(string key)
            => Keys.TryGetValue(key, out var value) ? value : string.Empty;
    }
}
