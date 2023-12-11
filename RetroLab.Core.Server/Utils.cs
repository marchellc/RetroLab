using Common.Logging;

using Newtonsoft.Json;

using RetroLab.API.Servers;

using ServerMod2;

using System;

namespace RetroLab
{
    public static class Utils
    {
        private static ServerListInfo listInfo = new ServerListInfo();

        private static DateTime lastConfigUpdate;

        private static string serverName;
        private static string serverPastebin;

        public static LogOutput Log;
        public static CustomNetworkManager Manager;

        public static ServerListInfo GetList()
        {
            Manager ??= (CustomNetworkManager)CustomNetworkManager.singleton;

            if ((DateTime.Now - lastConfigUpdate).TotalMinutes > 1)
            {
                lastConfigUpdate = DateTime.Now;

                serverName = Initializer.pluginManager.Server.Name;
                serverPastebin = ConfigFile.ServerConfig.GetString("serverinfo_pastebin_id");
            }

            listInfo.Port = Manager.networkPort;
            listInfo.Players = Manager.numPlayers - 1;
            listInfo.MaxPlayers = Manager.MaxPlayers();
            listInfo.Name = serverName;
            listInfo.Pastebin = serverPastebin;
            listInfo.Ip = CustomNetworkManager.Ip;

            return listInfo;
        }

        public static T Deserialize<T>(this string json)
            => JsonConvert.DeserializeObject<T>(json);

        public static string Serialize(this object obj)
            => JsonConvert.SerializeObject(obj);
    }
}