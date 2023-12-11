using Common.Logging;
using Common.Logging.File;

using HarmonyLib;

namespace RetroLab.Patches
{
    [HarmonyPatch(typeof(ServerRoles), nameof(ServerRoles.TargetSignServerChallenge))]
    public static class TargetSignServerChallengePatch
    {
        public static LogOutput Log { get; }

        static TargetSignServerChallengePatch()
        {
            Log = new LogOutput("ServerRoles::TargetSignServerChallenge");
            Log.AddLogger(Logger.Instance);
            Log.AddLogger(LogOutput.Common.GetLogger<FileLogger>());

            Log.Info("Patch initialized.");
        }

        public static bool Prefix(ServerRoles __instance, string challenge)
        {
            Log.Debug($"Server called: {challenge}");

            if (!CentralClient.IsConnected || CentralClient.Client is null)
            {
                Log.Error($"Server called for a challenge, but the central client is not connected!");
                return false;
            }

            if (!CentralClient.IsAuthed)
            {
                Log.Warn($"Central Client has not yet authentificated, downloading auth ..");

                CentralClient.DownloadAuth(auth => SendAuth(__instance));
                return false;
            }
            else
            {
                SendAuth(__instance);
                return false;
            }
        }

        public static void SendAuth(ServerRoles sr)
        {
            Log.Debug($"Sending authentification of {CentralClient.AuthResponse.Id} | {CentralClient.AuthResponse.Nick} | {CentralClient.AuthResponse.IsGlobalPerms}");
            sr.CallCmdServerSignatureComplete(CentralClient.AuthResponse.Id, CentralClient.AuthResponse.Nick, string.Empty, CentralClient.AuthResponse.IsGlobalPerms);
        }
    }
}