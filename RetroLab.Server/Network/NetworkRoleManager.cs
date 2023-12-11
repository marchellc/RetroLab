using Common.IO.Collections;

namespace RetroLab.Server.Network
{
    public static class NetworkRoleManager
    {
        public static LockedList<string> Moderators { get; } = new LockedList<string>();

        public static bool IsModerator(string id)
            => Moderators.Contains(id);
    }
}