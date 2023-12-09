using HarmonyLib;

using MEC;

using System.Collections.Generic;

using UnityEngine;

namespace RetroLab.Patches
{
    [HarmonyPatch(typeof(ServerListManager), nameof(ServerListManager.Refresh))]
    public static class RefreshListPatch
    {
        public static bool Prefix(ServerListManager __instance)
        {
            Timing.RunCoroutine(Refresh(__instance));
            return false;
        }

        private static IEnumerator<float> Refresh(ServerListManager sl)
        {
            foreach (var spawn in sl.spawns)
                Object.Destroy(spawn);

            sl.spawns.Clear();
            sl.loadingText.text = "";

            while (CentralClient.Client is null)
            {
                sl.loadingText.text = "The central server client is invalid.";
                yield return Timing.WaitForSeconds(1f);
            }

            sl.loadingText.text = "Downloading server list ..";

            CentralClient.Client.RequestList(servers =>
            {
                sl.loadingText.text = string.Empty;

                foreach (var server in servers)
                {
                    if (sl.filters != null && !sl.filters.AllowToSpawn(server.Info.Name))
                        continue;

                    var rec = sl.AddRecord().GetComponent<PlayButton>();

                    rec.Ip = server.Ip;
                    rec.Port = server.Port.ToString();
                    rec.InfoType = server.Info.Pastebin;
                    rec.Motd.text = server.Info.Name;
                    rec.Players.text = $"{server.Info.Players}/{server.Info.MaxPlayers}";
                }
            });
        }
    }
}
