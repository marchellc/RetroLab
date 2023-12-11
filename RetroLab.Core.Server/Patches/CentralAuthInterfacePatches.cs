using HarmonyLib;

namespace RetroLab.Patches
{
    [HarmonyPatch(typeof(CentralAuthInterface), nameof(CentralAuthInterface.Ok))]
    public static class OkPatch
    {
        public static bool Prefix(CentralAuthInterface __instance, string steamId, string nickname, string ban, bool bypass, bool DNT)
        {
            ServerConsole.AddLog($"Accepted authentification of user '{steamId}' with global ban status {ban}.");

            __instance._s.TargetConsolePrint(__instance._s.connectionToClient, $"Accepted your authentification (your ID: {steamId}) with global ban status {ban}", "green");

            var sr = __instance._s.GetComponent<ServerRoles>();
            var disableBypass = ConfigFile.ServerConfig.GetBool("disable_ban_bypass");

            if (disableBypass)
                bypass = false;

            if (DNT)
                sr.SetDoNotTrack();

            if ((!bypass || !ServerStatic.GetPermissionsHandler().IsVerified) && BanHandler.QueryBan(steamId, null).Key != null)
            {
                __instance._s.TargetConsolePrint(__instance._s.connectionToClient, "You are banned from this server.", "red");

                ServerConsole.AddLog("Player kicked due to local SteamID ban.");
                ServerConsole.Disconnect(__instance._s.connectionToClient, "You are banned from this server.");

                return false;
            }

            if ((!bypass || !ServerStatic.GetPermissionsHandler().IsVerified) && !WhiteList.IsWhitelisted(steamId))
            {
                __instance._s.TargetConsolePrint(__instance._s.connectionToClient, "You are not on the whitelist!", "red");

                ServerConsole.AddLog("Player kicked due to whitelist enabled.");
                ServerConsole.Disconnect(__instance._s.connectionToClient, "You are not on the whitelist for this server.");

                return false;
            }

            if ((ConfigFile.ServerConfig.GetBool("use_vac", true) || ServerStatic.PermissionsHandler.IsVerified) && ban != "0")
            {
                __instance._s.TargetConsolePrint(__instance._s.connectionToClient, "You have been globally banned: " + ban + ".", "red");

                ServerConsole.AddLog("Player kicked due to active global ban (" + ban + ").");
                ServerConsole.Disconnect(__instance._s.connectionToClient, "You have been globally banned: " + ban + ".");

                return false;
            }

            if (MuteHandler.QueryPersistantMute(steamId))
            {
                __instance._s.NetworkMuted = true;
                __instance._s.NetworkIntercomMuted = true;
                __instance._s.TargetConsolePrint(__instance._s.connectionToClient, "You are muted on the voice chat by the server administrator.", "red");
            }
            else if ((ConfigFile.ServerConfig.GetBool("global_mutes_voicechat", true) || ServerStatic.PermissionsHandler.IsVerified) && ban == "3")
            {
                __instance._s.NetworkMuted = true;
                __instance._s.NetworkIntercomMuted = true;
                __instance._s.TargetConsolePrint(__instance._s.connectionToClient, "You are globally muted on the voice chat.", "red");
            }
            else if (MuteHandler.QueryPersistantMute("ICOM-" + steamId))
            {
                __instance._s.NetworkIntercomMuted = true;
                __instance._s.TargetConsolePrint(__instance._s.connectionToClient, "You are muted on the intercom by the server administrator.", "red");
            }
            else if ((ConfigFile.ServerConfig.GetBool("global_mutes_intercom", true) || ServerStatic.PermissionsHandler.IsVerified) && ban == "4")
            {
                __instance._s.NetworkIntercomMuted = true;
                __instance._s.TargetConsolePrint(__instance._s.connectionToClient, "You are globally muted on the intercom.", "red");
            }

            sr.BypassStaff = sr.BypassStaff || bypass;

            if (sr.BypassStaff)
                __instance._s.TargetConsolePrint(__instance._s.connectionToClient, "You have the ban bypass flag, so you can't be banned from this server.", "cyan");

            sr.StartServerChallenge(0);

            return false;
        }
    }
}