using HarmonyLib;

namespace RetroLab.Patches
{
    [HarmonyPatch(typeof(CentralServer), nameof(CentralServer.Start))]
    public static class StartPatch
    {
        public static bool Prefix() => false;
    }

    [HarmonyPatch(typeof(CentralServer), nameof(CentralServer.RefreshServerList))]
    public static class RefreshPatch
    {
        public static bool Prefix() => false;
    }

    [HarmonyPatch(typeof(CentralServer), nameof(CentralServer.ChangeCentralServer))]
    public static class ChangePatch
    {
        public static bool Prefix() => false;
    }
}
