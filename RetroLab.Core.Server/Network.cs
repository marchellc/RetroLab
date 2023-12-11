using Common.Logging;

using Network.Tcp;

using System.Net;

using Network.Requests;
using Common.Logging.File;

namespace RetroLab
{
    public static class Network
    {
        public static LogOutput Log;
        public static TcpClient Client;

        public static void Load()
        {
            Log = new LogOutput("RetroLab.Network");
            Log.AddLogger(Logger.Instance);
            Log.AddLogger(LogOutput.Common.GetLogger<FileLogger>());

            Log.Info("Initializing network ..");

            if (!IPAddress.TryParse(Config.Instance.Ip, out var ip))
            {
                Log.Error($"Your IP address is invalid!");
                return;
            }

            Client = new TcpClient(new IPEndPoint(ip, Config.Instance.Port));

            Log.Info("TCP client initialized, starting ..");

            Client.Features.AddFeature<RequestManager>();
            Client.Features.AddFeature<CentralClient>();

            Client.Start();

            Log.Info("TCP client started!");
        }
    }
}