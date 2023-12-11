using HarmonyLib;

using MEC;

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace RetroLab.Patches
{
    [HarmonyPatch(typeof(ServerListManager), nameof(ServerListManager.Refresh))]
    public static class RefreshListPatch
    {
        public static bool Prefix(ServerListManager __instance)
        {
            if (!SceneManager.GetActiveScene().name.Contains("Menu"))
                return false;

            Timing.RunCoroutine(Refresh(__instance));
            return false;
        }

        private static IEnumerator<float> Refresh(ServerListManager sl)
        {
            if (!SceneManager.GetActiveScene().name.Contains("Menu"))
                yield break;

            foreach (var spawn in sl.spawns)
                Object.Destroy(spawn);

            sl.spawns.Clear();
            sl.loadingText.text = "Downloading server list ..";

            while (CentralClient.Client is null || !CentralClient.IsConnected || CentralClient.Requests is null)
            {
                sl.loadingText.text = "The central server client is not connected";
                yield return Timing.WaitForSeconds(1f);
            }

            sl.loadingText.text = "";

            CentralClient.DownloadServerList(_ =>
            {
                if (!SceneManager.GetActiveScene().name.Contains("Menu"))
                    return;

                foreach (var server in CentralClient.Servers)
                    sl.TrySpawnRecord(server);
            });
        }
    }
}
