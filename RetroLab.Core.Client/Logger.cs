using Common.Logging;

using System;

using UnityEngine;

namespace RetroLab
{
    public class Logger : Common.Logging.ILogger
    {
        public static Logger Instance { get; } = new Logger();

        public static readonly Color32 Red = new Color32(byte.MaxValue, 0, 0, byte.MaxValue);
        public static readonly Color32 Green = new Color32(193, byte.MaxValue, 51, byte.MaxValue);
        public static readonly Color32 Yellow = new Color32(byte.MaxValue, 246, 51, byte.MaxValue);
        public static readonly Color32 Cyan = new Color32(51, 252, byte.MaxValue, byte.MaxValue);

        private LogMessage last;

        public LogMessage Latest => last;
        public DateTime Started { get; }

        public Logger()
        {
            Started = DateTime.Now;
        }

        public void Emit(LogMessage message)
        {
            last = message;

            var msg = message.GetString(false, false);

            Debug.Log(msg);

            switch (message.Level)
            {
                case LogLevel.Fatal:
                case LogLevel.Error:
                    WriteConsole(msg, Red);
                    break;

                case LogLevel.Warning:
                    WriteConsole(msg, Yellow);
                    break;

                case LogLevel.Information:
                    WriteConsole(msg, Green);
                    break;

                case LogLevel.Verbose:
                case LogLevel.Trace:
                case LogLevel.Debug:
                    WriteConsole(msg, Cyan);
                    break;
            }
        }

        public static void WriteConsole(string txt, Color32 color)
            => GameConsole.Console.singleton?.AddLog(txt, color);
    }
}