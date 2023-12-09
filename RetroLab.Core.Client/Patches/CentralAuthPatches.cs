using HarmonyLib;

using System.Collections.Generic;

using MEC;

namespace RetroLab.Patches
{
    [HarmonyPatch(typeof(CentralAuth), nameof(CentralAuth.GenerateToken))]
    public static class GenerateTokenPatch
    {
        public static bool Prefix(CentralAuth __instance, ICentralAuth icaa)
        {
            __instance._responded = true;
            __instance._ica = icaa;

            return false;
        }
    }

    [HarmonyPatch(typeof(CentralAuth), nameof(CentralAuth.Update))]
    public static class UpdatePatch
    {
        public static bool Prefix(CentralAuth __instance)
        {
            if (__instance._responded)
            {
                __instance._responded = false;
                Timing.RunCoroutine(RequestToken(__instance), Segment.FixedUpdate);
            }

            if (!string.IsNullOrEmpty(__instance._roleToRequest) && PlayerManager.localPlayer != null && !string.IsNullOrEmpty(PlayerManager.localPlayer.GetComponent<NicknameSync>().myNick))
            {
                GameConsole.Console.singleton.AddLog("Requesting your global badge...", UnityEngine.Color.yellow, false);

                __instance._ica.RequestBadge(__instance._roleToRequest);
                __instance._roleToRequest = string.Empty;
            }

            return false;
        }

        private static IEnumerator<float> RequestToken(CentralAuth ca)
        {
            while (CentralClient.Client is null)
            {
                Network.Log.Warn($"Cannot authentificate - client is null!");
                yield return Timing.WaitForSeconds(1f);
            }

            while (CentralClient.Token is null)
            {
                Network.Log.Warn($"Cannot authentificate - missing token!");
                yield return Timing.WaitForSeconds(1f);
            }

            Network.Log.Info($"Calling CentralAuthInterface::TokenGenerated with token ID {CentralClient.Token.Id}");

            ca._ica.TokenGenerated(CentralClient.Token.Id);
        }
    }
}
