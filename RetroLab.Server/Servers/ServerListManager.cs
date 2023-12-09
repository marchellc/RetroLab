using Common.IO.Collections;

using RetroLab.API.Servers;
using RetroLab.Server.Core;
using System.Net;

namespace RetroLab.Server.Servers
{
    public static class ServerListManager
    {
        public static LockedList<ServerInfo> Servers { get; } = new LockedList<ServerInfo>();
        public static LockedList<string> Verified { get; set; }

        public static void Enable()
        {
            Verified = new LockedList<string>(Paths.GetJson(Paths.Servers, "verified.json", new List<string>() { "127.0.0.1" }));

            Commands.Create("verify", args =>
            {
                if (args.Length != 1)
                    return "Invalid usage! (verify ip)";

                var ip = args[0].Trim();

                if (!IPAddress.TryParse(ip, out _))
                    return "Invalid IP!";

                if (Verified.Contains(ip))
                    return $"That IP is already verified!";

                Verified.Add(ip);

                Save();

                return $"Verified IP: {ip}";
            });

            Commands.Create("unverify", args =>
            {
                if (args.Length != 1)
                    return "Invalid usage! (verify ip)";

                var ip = args[0].Trim();

                if (!IPAddress.TryParse(ip, out _))
                    return "Invalid IP!";

                if (!Verified.Contains(ip))
                    return $"That IP is not verified!";

                Verified.Remove(ip);

                Save();

                return $"Unverified IP: {ip}";
            });
        }

        public static void Save()
            => Paths.Write(Paths.Servers, "verified.json", Verified.ToList());

        public static bool IsVerified(string ip)
            => Verified.Contains(ip);

        public static List<ServerInfo> GetVerified()
            => Servers.Where(s => IsVerified(s.Ip)).ToList();

        public static void AddServer(string ip, int port, ServerListInfo serverListInfo)
        {
            if (Servers.Any(s => s.Ip == ip && s.Port == port))
                return;

            Servers.Add(new ServerInfo
            {
                Info = serverListInfo,

                Ip = ip,
                Port = port
            });
        }

        public static void RemoveServer(string ip, int port)
            => Servers.RemoveRange(s => s.Ip == ip && s.Port == port);

        public static ServerListUpdateResult ProcessUpdate(string ip, int port, ServerListUpdateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Info.Name)
                || string.IsNullOrWhiteSpace(request.Info.Pastebin)
                || request.Info.MaxPlayers <= 0
                || request.Info.Players < 0)
                return ServerListUpdateResult.MissingInfo;

            foreach (var server in Servers)
            {
                if (server.Ip == ip && server.Port == port)
                {
                    server.Info = request.Info;
                    return ServerListUpdateResult.Ok;
                }
            }

            return ServerListUpdateResult.NotVerified;
        }
    }
}