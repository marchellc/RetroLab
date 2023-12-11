using Common.Logging;
using Common.Logging.File;

using HarmonyLib;

using RetroLab.API.Authentification;

namespace RetroLab.Patches
{
    [HarmonyPatch(typeof(CentralAuth), nameof(CentralAuth.StartValidateToken))]
    public static class StartValidateTokenPatch
    {
        public static LogOutput Log { get; }

        static StartValidateTokenPatch()
        {
            Log = new LogOutput("CentralAuth::StartValidateToken");
            Log.AddLogger(Logger.Instance);
            Log.AddLogger(LogOutput.Common.GetLogger<FileLogger>());

            Log.Info("Initialized.");
        }

        public static bool Prefix(ICentralAuth icaa, string token)
        {
            Log.Debug($"Validating token: {token}");

            if (!CentralClient.IsConnected || !CentralClient.IsRequested)
            {
                Fail(icaa, "This server's central client is not connected and your token cannot be validated.");
                return false;
            }

            CentralClient.ValidateAuthentification(token, res =>
            {
                Log.Debug($"Received validation of {token}: {res.Result}");

                switch (res.Result)
                {
                    case AuthValidationResult.InvalidId:
                        Fail(icaa, "The central server marked your ID as invalid.");
                        break;

                    case AuthValidationResult.MissingId:
                        Fail(icaa, "Your client sent an empty ID.");
                        break;

                    case AuthValidationResult.Ok:
                        icaa.Ok(res.Id, res.Nick, res.IsGlobalBan ? "1" : "0", string.Empty, string.Empty, res.IsGlobalPerms, false);
                        break;
                }
            });

            return false;
        }

        public static void Fail(ICentralAuth icaa, string reason)
        {
            Log.Error(reason);

            icaa.GetCcm().TargetConsolePrint(icaa.GetCcm().connectionToClient, reason, "red");
            icaa.FailToken(reason);
        }
    }
}