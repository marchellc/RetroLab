using HarmonyLib;

namespace RetroLab
{
    [HarmonyPatch(typeof(DiscordController), nameof(DiscordController.ReadyCallback))]
    public static class DiscordClient
    {
        public static string Id;
        public static string Name;

        public static ulong ClearId;

        public static bool IsReady;

        public static bool Prefix(DiscordController __instance, DiscordRpc.DiscordUser connectedUser)
        {
            Id = connectedUser.userId;
            Name = connectedUser.username;
            ClearId = ulong.Parse(connectedUser.userId);
            IsReady = true;

            Network.Load();

            Loader.Log.Info($"Discord connected! ({connectedUser.username}) ({connectedUser.userId})");

            return true;
        }
    }
}
