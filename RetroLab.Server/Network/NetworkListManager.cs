using Common.IO.Collections;
using Common.Logging;
using Common.Pooling.Pools;

using RetroLab.API.Servers;
using RetroLab.Server.Core;

using System.Net;

namespace RetroLab.Server.Network
{
    public static class NetworkListManager
    {
        private static Timer saveTimer;

        public static LockedList<string> VerifiedServers { get; } = new LockedList<string>();
        public static LogOutput Log { get; private set; }

        public static void Enable()
        {
            saveTimer?.Dispose();

            Log?.Dispose();

            Log = new LogOutput("RetroLab.ServerList");
            Log.Setup();

            Commands.Create("verification", SetStatusCommand);

            Log.Info($"Initializing the server list ..");

            VerifiedServers.Clear();
            VerifiedServers.AddRange(Paths.GetJson(Paths.Net, "verified.json", new List<string>() { "127.0.0.1" }));

            saveTimer = new Timer(SaveServers, null, 0, 5000);

            Log.Info($"Loaded {VerifiedServers.Count} verified server(s) from the cache.");
        }

        public static bool IsVerified(string ip)
            => VerifiedServers.Contains(ip);

        public static bool SetVerified(string ip, bool status)
        {
            Log.Debug($"Setting verification status of {ip} to {status}");

            var isVerified = VerifiedServers.Contains(ip);

            if (isVerified && status)
            {
                Log.Warn($"IP '{ip}' is already verified!");
                return false;
            }

            if (!isVerified && !status)
            {
                Log.Warn($"IP '{ip}' is not verified!");
                return false;
            }

            if (status)
            {
                VerifiedServers.Add(ip);
                SendUpdate(ip, status);
                SaveServers(null);
                Log.Info($"Verified server IP {ip}");
                return true;
            }
            else if (VerifiedServers.Remove(ip))
            {
                SendUpdate(ip, status);
                SaveServers(null);
                Log.Info($"Removed verification of server IP {ip}");
                return true;
            }
            else
            {
                Log.Error("Failed!");
                return false;
            }
        }

        public static void SendUpdate(string ip, bool update)
        {
            Log.Debug($"Attempting to send verification update to server '{ip}' ({update})");

            foreach (var handler in NetworkHandler.Handlers)
            {
                if (!handler.IsServer || !handler.IsLoaded || handler.List is null)
                    continue;

                if (string.IsNullOrWhiteSpace(handler.Ip))
                    continue;

                if (handler.Ip != ip)
                    continue;

                Log.Debug($"Found server: {handler.Ip}:{handler.Port}, sending update.");

                handler.SendVerificationUpdate(update);
            }
        }

        public static ServerListInfo[] GetServers()
        {
            var servers = ListPool<ServerListInfo>.Shared.Next();

            foreach (var handler in NetworkHandler.Handlers)
            {
                if (!handler.IsServer || handler.List is null || !handler.IsLoaded)
                    continue;

                servers.Add(handler.List.Value);
            }

            return ListPool<ServerListInfo>.Shared.ToArrayReturn(servers);
        }

        private static void SaveServers(object _)
            => Paths.Write(Paths.Net, "verified.json", VerifiedServers.ToList());

        private static string SetStatusCommand(string[] args)
        {
            if (args.Length != 2)
                return "Invalid command usage. 'verification (server IP) (verification status)'";

            if (!IPAddress.TryParse(args[0], out _))
                return "Failed to parse server IP value.";

            if (!bool.TryParse(args[1], out var status))
                return "Failed to parse verification status value.";

            if (!SetVerified(args[0], status))
                return $"Failed to set status; that IP most likely already has that status.";

            return $"Set verification status of '{args[0]}' to {status}";
        }
    }
}