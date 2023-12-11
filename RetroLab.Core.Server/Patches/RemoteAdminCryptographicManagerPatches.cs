using HarmonyLib;

using RemoteAdmin;

namespace RetroLab.Patches
{
    [HarmonyPatch(typeof(RemoteAdminCryptographicManager), nameof(RemoteAdminCryptographicManager.CallTargetDiffieHellmanExchange))]
    public static class CallTargetDiffieHellmanExchangePatch
    {
        public static bool Prefix() => false;
    }

    [HarmonyPatch(typeof(RemoteAdminCryptographicManager), nameof(RemoteAdminCryptographicManager.Init))]
    public static class InitPatch
    {
        public static bool Prefix() => false;
    }

    [HarmonyPatch(typeof(RemoteAdminCryptographicManager), nameof(RemoteAdminCryptographicManager.StartExchange))]
    public static class StartExchangePatch
    {
        public static bool Prefix() => false;
    }
}