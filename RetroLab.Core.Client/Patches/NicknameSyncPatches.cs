using HarmonyLib;

using UnityEngine;
using UnityEngine.UI;

namespace RetroLab.Patches
{
    [HarmonyPatch(typeof(NicknameSync), nameof(NicknameSync.Start))]
    public static class NicknameSyncStartPatch
    {
        public static bool Prefix(NicknameSync __instance)
        {
            __instance._role = __instance.GetComponent<ServerRoles>();
            __instance.spectCam = __instance.GetComponent<Scp049PlayerScript>().plyCam.transform;

            __instance.n_text = GameObject.Find("Nickname Text").GetComponent<Text>();

            __instance.CallCmdSetNick(DiscordClient.Name);

            return false;
        }
    }
}