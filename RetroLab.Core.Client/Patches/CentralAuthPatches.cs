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
    public static class AuthUpdatePatch
    {
        public static bool Prefix(CentralAuth __instance)
        {
            if (__instance._responded)
            {
                __instance._responded = false;
                Timing.RunCoroutine(RequestToken(__instance), Segment.FixedUpdate);
            }

            return false;
        }

        private static IEnumerator<float> RequestToken(CentralAuth ca)
        {
            while (!CentralClient.IsConnected || CentralClient.Client is null)
            {
                Network.Log.Warn($"Cannot authentificate - client is not connected!");
                yield return Timing.WaitForSeconds(1f);
            }

            while (!DiscordClient.IsReady)
            {
                Network.Log.Warn($"Cannot authentificate - Discord has not been loaded!");
                yield return Timing.WaitForSeconds(1f);
            }

            Network.Log.Info($"Calling CentralAuthInterface::TokenGenerated with token ID {DiscordClient.Id}");

            ca._ica.TokenGenerated(DiscordClient.Id);
        }
    }
}
