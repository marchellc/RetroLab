using HarmonyLib;

namespace RetroLab.Patches
{
    [HarmonyPatch(typeof(AchievementManager), nameof(AchievementManager.StatsProgress))]
    public static class AchievementPatches
    {
        public static bool Prefix() => false;
    }
}