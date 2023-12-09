using Common.Logging;

using Network.Interfaces.Controllers;
using Network.Tcp;

using System.Net;

using Network.Requests;

namespace RetroLab
{
    public static class Network
    {
        public static LogOutput Log;

        public static TcpClient Client;
        public static TcpPeer Peer;

        public static void Load()
        {
            Log = new LogOutput("RetroLab.Network");
            Log.Setup();

            Log.Info("Initializing network ..");

            if (!IPAddress.TryParse(Config.Instance.Ip, out var ip))
            {
                Log.Error($"Your IP address is invalid!");
                return;
            }

            Client = new TcpClient(new IPEndPoint(ip, Config.Instance.Port));

            Client.Features.AddFeature<RequestManager>();
            Client.Features.AddFeature<CentralClient>();

            Client.OnConnected += OnConnected;
            Client.OnDisconnected += OnDisconnected;

            Client.Start();
        }

        private static void OnDisconnected(IPeer peer)
        {
            if (Peer is null)
                return;

            Peer = null;

            Utils.Disconnect("Disconnected from the central server!");
        }

        private static void OnConnected(IPeer peer)
        {
            Peer = (TcpPeer)peer;

            Log.Info($"Connected to the central server! ({peer.Target})");
        }
    }
}
