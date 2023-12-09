using Common.Logging;

using RemoteAdmin;

using System;

namespace RetroLab
{
    public static class Utils
    {
        public static LogOutput Log;

        public static CharacterClassManager Ccm;
        public static QueryProcessor Qp;
        public static NicknameSync Ns;

        public static void SetComponents()
        {
            Ccm = PlayerManager.localPlayer.GetComponent<CharacterClassManager>();
            Qp = PlayerManager.localPlayer.GetComponent<QueryProcessor>();
            Ns = PlayerManager.localPlayer.GetComponent<NicknameSync>();
        }

        public static bool Disconnect(string reason = "Unspecified reason.")
        {
            if (PlayerManager.localPlayer != null)
            {
                try
                {
                    Log.Info($"Disconnecting from server ..");

                    var ccm = PlayerManager.localPlayer.GetComponent<CharacterClassManager>();

                    if (ccm is null)
                    {
                        Log.Error($"Failed to disconnect; CCM is null");
                        return false;
                    }

                    if (ccm.connectionToServer is null)
                    {
                        Log.Error($"Failed to disconnect; server connection is null");
                        return false;
                    }

                    ccm.DisconnectClient(ccm.connectionToServer, reason);

                    Log.Info("Disconnected!");

                    return true;
                }
                catch (Exception ex)
                {
                    Log.Error($"An error occured while disconnecting client:\n{ex}");
                }
            }
            else
            {
                Log.Warn($"Cannot disconnect; local player is null");
            }

            return false;
        }
    }
}
