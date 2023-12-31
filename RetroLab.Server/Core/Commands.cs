﻿using Common.IO.Collections;
using Common.Logging;
using Common.Reflection;
using Common.Logging.File;
using Common.Logging.Console;

namespace RetroLab.Server.Core
{
    public static class Commands
    {
        private static Timer timer;
        private static LogOutput log;
        private static LockedDictionary<string, Func<string[], string>> commands = new LockedDictionary<string, Func<string[], string>>();

        public static void Enable()
        {
            timer = new Timer(OnUpdate, null, 0, 500);

            log = new LogOutput("Commands");

            log.AddLogger(new FileLogger(LogUtils.GetFilePath("Commands")));
            log.AddLogger(new ConsoleLogger());

            log.Info("Commands enabled.");
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

            log.Info($">>> {cmd.ToUpper()}");

            if (!commands.TryGetValue(cmd, out var callback))
            {
                log.Error(">>> No such command.");
                return;
            }

            var output = callback.Call(split.Skip(1).ToArray(), ex => log.Error($"Command execution failed!\n{ex}"));

            if (string.IsNullOrWhiteSpace(output))
            {
                log.Warn(">>> No output from command.");
                return;
            }

            log.Info($">>> {output}");
        }
    }
}