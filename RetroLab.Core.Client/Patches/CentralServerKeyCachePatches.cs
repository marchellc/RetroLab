using HarmonyLib;

namespace RetroLab.Patches
{
    [HarmonyPatch(typeof(CentralServerKeyCache), nameof(CentralServerKeyCache.ReadCache))]
    public static class ReadCachePatch
    {
        public static bool Prefix() => false;
    }

    [HarmonyPatch(typeof(CentralServerKeyCache), nameof(CentralServerKeyCache.SaveCache))]
    public static class SaveCachePatch
    {
        public static bool Prefix() => false;
    }
}
