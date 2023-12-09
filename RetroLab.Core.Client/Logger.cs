using Common.Logging;

using System;

using UnityEngine;

namespace RetroLab
{
    public class Logger : Common.Logging.ILogger
    {
        public static Logger Instance;

        private LogMessage last;

        public LogMessage Latest => last;
        public DateTime Started { get; }

        public bool IsUnity { get; set; }

        public Logger(bool unity)
        {
            IsUnity = unity;
            Started = DateTime.Now;
            Instance = this;
        }

        public void Emit(LogMessage message)
        {
            last = message;

            if (IsUnity)
            {
                switch (message.Level)
                {
                    case LogLevel.Fatal:
                    case LogLevel.Error:
                        Debug.LogError(message.GetString());
                        break;

                    case LogLevel.Warning:
                        Debug.LogWarning(message.GetString());
                        break;

                    default:
                        Debug.Log(message.GetString());
                        break;
                }
            }

            ServerConsole.AddLog(message.GetString(false));
        }
    }
}