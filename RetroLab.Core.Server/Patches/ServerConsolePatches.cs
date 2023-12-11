using HarmonyLib;

namespace RetroLab.Patches
{
    [HarmonyPatch(typeof(ServerConsole), nameof(ServerConsole.RefreshToken))]
    public static class RefreshTokenPatch
    {
        public static bool Prefix() => false;
    }

    [HarmonyPatch(typeof(ServerConsole), nameof(ServerConsole.RunRefreshCentralServers))]
    public static class RunRefreshCentralServersPatch
    {
        public static bool Prefix() => false;
    }

    [HarmonyPatch(typeof(ServerConsole), nameof(ServerConsole.RunRefreshPublicKey))]
    public static class RunRefreshPublicKeyPatch
    {
        public static bool Prefix() => false;
    }

    [HarmonyPatch(typeof(ServerConsole), nameof(ServerConsole.RunServer))]
    public static class RunServerPatch
    {
        public static bool Prefix() => false;
    }
}