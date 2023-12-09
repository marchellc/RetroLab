using HarmonyLib;

namespace RetroLab.Patches
{
    [HarmonyPatch(typeof(SteamManager), nameof(SteamManager.SteamId64), MethodType.Getter)]
    public static class SteamId64GetterPatch
    {
        public static bool Prefix(ref ulong __result)
        {
            __result = DiscordClient.ClearId;
            return false;
        }
    }

    [HarmonyPatch(typeof(SteamManager), nameof(SteamManager.Running), MethodType.Getter)]
    public static class RunningGetterPatch
    {
        public static bool Prefix(ref bool __result)
        {
            __result = true;
            return false;
        }
    }

    [HarmonyPatch(typeof(SteamManager), nameof(SteamManager.CancelTicket))]
    public static class CancelTicketPatch
    {
        public static bool Prefix() => false;
    }

    [HarmonyPatch(typeof(SteamManager), nameof(SteamManager.GetPersonaName))]
    public static class GetPersonaNamePatch
    {
        public static bool Prefix(ulong steamid, ref string __result)
        {
            __result = DiscordClient.Name;
            return false;
        }
    }

    [HarmonyPatch(typeof(SteamManager), nameof(SteamManager.OpenProfile))]
    public static class OpenProfilePatch
    {
        public static bool Prefix() => false;
    }

    [HarmonyPatch(typeof(SteamManager), nameof(SteamManager.StopClient))]
    public static class StopClientPatch
    {
        public static bool Prefix() => false;
    }

    [HarmonyPatch(typeof(SteamManager), nameof(SteamManager.CheckAchievement))]
    public static class CheckAchievementPatch
    {
        public static bool Prefix(string key, ref bool __result)
        {
            __result = true;
            return false;
        }
    }
}
