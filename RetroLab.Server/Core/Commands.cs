using Common.IO.Collections;
using Common.Logging;
using Common.Extensions;

namespace RetroLab.Server.Core
{
    public static class Commands
    {
        private static Timer timer;
        private static LockedDictionary<string, Func<string[], string>> commands = new LockedDictionary<string, Func<string[], string>>();

        public static void Enable()
        {
            timer = new Timer(OnUpdate, null, 0, 500);
        }

        public static void Create(string cmd, Func<string[], string> callback)
            => commands[cmd] = callback;

        public static void OnUpdate(object _)
        {
            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
                return;

            var split = input.Split(' ');

            if (split.Length <= 0)
                return;

            var cmd = split[0].ToLower();

            LogOutput.Common.Raw($">>> {cmd.ToUpper()}", ConsoleColor.Magenta);

            if (!commands.TryGetValue(cmd, out var callback))
            {
                LogOutput.Common.Raw(">>> No such command.", ConsoleColor.Red);
                return;
            }

            var output = callback.Call(split.Skip(1).ToArray());

            if (string.IsNullOrWhiteSpace(output))
            {
                LogOutput.Common.Raw(">>> No output from command.", ConsoleColor.Green);
                return;
            }

            LogOutput.Common.Raw($">>> {output}", ConsoleColor.Blue);
        }
    }
}