using Common.Logging;
using Common.Logging.File;

using HarmonyLib;

using RemoteAdmin;

using System;

namespace RetroLab.Patches
{
    [HarmonyPatch(typeof(ServerRoles), nameof(ServerRoles.StartServerChallenge))]
    public static class StartServerChallengePatch
    {
        public static LogOutput Log { get; }

        static StartServerChallengePatch()
        {
            Log = new LogOutput("ServerRoles::StartServerChallenge");
            Log.AddLogger(Logger.Instance);
            Log.AddLogger(LogOutput.Common.GetLogger<FileLogger>());

            Log.Info("Initialized.");
        }

        public static bool Prefix(ServerRoles __instance, int selector)
        {
            Log.Debug($"Requesting server auth challenge");

            __instance.CallTargetSignServerChallenge(__instance.connectionToClient, string.Empty);
            return false;
        }
    }

    [HarmonyPatch(typeof(ServerRoles), nameof(ServerRoles.CmdServerSignatureComplete))]
    public static class CmdServerSignatureCompletePatch
    {
        public static LogOutput Log { get; }

        static CmdServerSignatureCompletePatch()
        {
            Log = new LogOutput("ServerRoles::CmdServerSignatureComplete");
            Log.AddLogger(Logger.Instance);

            Log.Info("Initialized.");
        }

        public static bool Prefix(ServerRoles __instance, string challenge, string response, bool hide)
        {
            try
            {
                Log.Debug($"Received challenge response: {challenge} ({response}) ({hide})");

                __instance.GetComponent<CharacterClassManager>().NetworkSteamId = challenge;
                __instance.GetComponent<NicknameSync>().UpdateNickname(response);

                if (__instance.DoNotTrack)
                    __instance.LogDNT();

                __instance.RefreshPermissions();

                if (hide && ServerStatic.PermissionsHandler.StaffAccess)
                {
                    __instance.NetworkMyText = "Global Moderator";
                    __instance.NetworkMyColor = "gold";
                    __instance.Permissions = ServerStatic.PermissionsHandler.FullPerm;
                    __instance.HiddenBadge = string.Empty;
                    __instance.GlobalSet = true;
                    __instance.GlobalBadgeType = 4;
                    __instance.CallRpcResetFixed();
                    __instance.OverwatchPermitted = true;
                    __instance.Staff = true;
                    __instance.RaEverywhere = true;
                    __instance.RemoteAdmin = true;
                    __instance.RemoteAdminMode = ServerRoles.AccessMode.GlobalAccess;
                    __instance.GetComponent<QueryProcessor>().PasswordTries = 0;
                    __instance.CallTargetOpenRemoteAdmin(__instance.connectionToClient, false);
                    __instance.GetComponent<CharacterClassManager>().TargetConsolePrint(__instance.connectionToClient, "Your remote admin access has been granted (global permissions - staff).", "cyan");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return false;
        }
    }
}