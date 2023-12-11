using Common.Logging;

using System;

using UnityEngine;

namespace RetroLab
{
    public class Logger : Common.Logging.ILogger
    {
        public static Logger Instance { get; } = new Logger();

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

            var msg = message.GetString(false);

            Debug.Log(msg);
            ServerConsole.AddLog(msg);
        }
    }
}