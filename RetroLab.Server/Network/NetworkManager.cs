using Common.Logging;

using Network.Requests;
using Network.Tcp;

using RetroLab.Server.Core;

using System.Net;

namespace RetroLab.Server.Network
{
    public static class NetworkManager
    {
        public static LogOutput Log;
        public static NetworkConfig Config;
        public static TcpServer Server;

        public static void Load()
        {
            Log = new LogOutput("RetroLab.Network");
            Log.Setup();

            Config = Paths.GetJson(Paths.Net, "config.json", new NetworkConfig());

            Program.OnExiting += OnExit;

            Log.Info($"Initializing the TCP server on: {Config.Port}");

            Server = new TcpServer(new IPEndPoint(IPAddress.Any, Config.Port));

            Server.Features.AddFeature<RequestManager>();
            Server.Features.AddFeature<NetworkHandler>();

            Log.Info("Starting the TCP server ..");

            Server.Start();
        }

        private static void OnExit()
        {
            Program.OnExiting -= OnExit;

            Log.Dispose();
            Log = null;

            Config = null;
        }
    }
}
