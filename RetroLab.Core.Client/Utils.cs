using Common.Logging;

using RetroLab.API.Servers;

using System;
using System.Net;

namespace RetroLab
{
    public static class Utils
    {
        public static LogOutput Log;

        public static bool TrySpawnRecord(this ServerListManager sl, ServerListInfo server)
        {
            try
            {
                if (sl is null)
                {
                    Log.Error($"Cannot spawn server record; server list manager is null.");
                    return false;
                }

                Log.Debug($"Spawning server record ({server.Ip ?? "MISSING IP"}:{server.Port}): {server.Name ?? "MISSING NAME"} | {server.Pastebin ?? "MISSING PASTEBIN"} | {server.Players}/{server.MaxPlayers}");

                if (string.IsNullOrWhiteSpace(server.Ip) || !IPAddress.TryParse(server.Ip, out _))
                {
                    Log.Error($"Cannot spawn server record; invalid server IP");
                    return false;
                }

                if (server.Port <= 0 || server.Port >= (short.MaxValue * 2))
                {
                    Log.Error($"Cannot spawn server record; invalid server port");
                    return false;
                }

                if (string.IsNullOrWhiteSpace(server.Name))
                {
                    Log.Error($"Cannot spawn server record; invalid server name");
                    return false;
                }

                if (string.IsNullOrWhiteSpace(server.Pastebin))
                {
                    Log.Error($"Cannot spawn server record; invalid pastebin ID");
                    return false;
                }

                if (server.MaxPlayers <= 0)
                {
                    Log.Error($"Cannot spawn server record; invalid max player count");
                    return false;
                }

                if (server.Players < 0)
                {
                    Log.Error($"Cannot spawn server record; invalid player count");
                    return false;
                }

                if (sl.filters != null && !sl.filters.AllowToSpawn(server.Name))
                {
                    Log.Warn($"Cannot spawn server record; blacklisted by nickname filter");
                    return true;
                }

                var rec = sl.AddRecord();

                if (rec is null)
                {
                    Log.Error($"Failed to spawn server record");
                    return false;
                }

                var button = rec.GetComponent<PlayButton>();

                if (button is null)
                {
                    Log.Error($"Failed to retrieve PlayButton component");
                    return false;
                }

                button.Ip = server.Ip;
                button.Port = server.Port.ToString();
                button.InfoType = server.Pastebin;
                button.Motd.text = server.Name;
                button.Players.text = $"{server.Players}/{server.MaxPlayers}";

                sl.spawns.Add(rec);

                Log.Debug($"Spawned a server record for {server.Ip}:{server.Port} ({server.Name})");

                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to spawn server record!\n{ex}");
                return false;
            }
        }

        public static bool Disconnect(string reason = "Unspecified reason.")
        {
            if (PlayerManager.localPlayer != null)
            {
                try
                {
                    Log.Info($"Disconnecting from server ..");

                    var ccm = PlayerManager.localPlayer.GetComponent<CharacterClassManager>();

                    if (ccm is null)
                    {
                        Log.Error($"Failed to disconnect; CCM is null");
                        return false;
                    }

                    if (ccm.connectionToServer is null)
                    {
                        Log.Error($"Failed to disconnect; server connection is null");
                        return false;
                    }

                    ccm.DisconnectClient(ccm.connectionToServer, reason);

                    Log.Info("Disconnected!");

                    return true;
                }
                catch (Exception ex)
                {
                    Log.Error($"An error occured while disconnecting client:\n{ex}");
                }
            }
            else
            {
                Log.Warn($"Cannot disconnect; local player is null");
            }

            return false;
        }
    }
}
