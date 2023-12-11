using Common.IO.Collections;

namespace RetroLab.Server.Network
{
    public static class NetworkBanManager
    {
        public static LockedList<string> Bans { get; } = new LockedList<string>();

        public static bool IsBanned(string id)
            => Bans.Contains(id);
    }
}