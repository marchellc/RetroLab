using Common.IO.Collections;

using Network.Extensions;
using Network.Interfaces.Controllers;
using Network.Interfaces.Requests;
using Network.Tcp;

using RetroLab.API.Authentification;
using RetroLab.API.Players;
using RetroLab.API.Servers;

namespace RetroLab.Server.Network
{
    public static class NetworkConnectionHandler
    {
        private static LockedDictionary<IPeer, DateTime> pendingAuth = new LockedDictionary<IPeer, DateTime>();
        private static Timer timer;

        public static int AuthTimeout = 1500;

        public static void Prepare(TcpServer server)
        {
            timer = new Timer(OnUpdate, null, 0, 150);

            server.OnConnected += OnConnected;
            server.OnDisconnected += OnDisconnected;
        }

        public static void Stop(TcpServer server)
        {
            server.OnConnected -= OnConnected;
            server.OnDisconnected -= OnDisconnected;

            timer.Dispose();
            timer = null;

            pendingAuth.Clear();
        }

        private static void OnConnected(IPeer peer)
        {
            pendingAuth[peer] = DateTime.Now;

            peer.Transport.CreateHandler<ServerAuthResponse>(msg => OnServerAuth(peer, msg));
            peer.Transport.CreateHandler<PlayerAuthResponse>(msg => OnClientAuth(peer, msg));

            peer.Transport.Send(new AuthRequest());
        }

        private static void OnDisconnected(IPeer peer)
        {
            pendingAuth.Remove(peer);
        }

        private static void OnServerAuth(IPeer peer, ServerAuthResponse msg)
        {
            pendingAuth.Remove(peer);
            peer.Features.AddFeature<NetworkServerHandler>().Initialize(msg);
            NetworkHandler.Log.Info($"Peer '{peer.Target}' authentificated as a server! (IP: {msg.Ip}; Port: {msg.Port})");
        }

        private static void OnClientAuth(IPeer peer, PlayerAuthResponse msg)
        {
            pendingAuth.Remove(peer);
            peer.Features.AddFeature<NetworkPlayerHandler>().Initialize(msg);
            NetworkHandler.Log.Info($"Peer '{peer.Target}' authentificated as a player! (ID: {msg.Id}; Name: {msg.Name})");
        }

        private static void OnUpdate(object _)
        {
            if (pendingAuth.Count <= 0)
                return;

            foreach (var key in pendingAuth.Keys)
            {
                if ((DateTime.Now - pendingAuth[key]).TotalMilliseconds >= AuthTimeout)
                {
                    NetworkHandler.Log.Warn($"Peer '{key.Target}' failed to authentificate in time!");
                    key.Disconnect();
                }
            }
        }
    }
}