using Common.Logging;
using Common.Logging.File;
using Common.Reflection;
using MEC;
using Network.Features;
using Network.Requests;

using RetroLab.API.Authentification;
using RetroLab.API.Servers;

using System;
using System.Threading;

namespace RetroLab
{
    public class CentralClient : Feature
    {
        public static CentralClient Client;
        public static RequestManager Requests;

        public static bool IsConnected;
        public static bool IsVerified;
        public static bool IsRequested;

        private Timer updateTimer;

        public override void OnStarted()
        {
            base.OnStarted();

            Client = this;

            Requests = Peer.Features.GetFeature<RequestManager>();

            Transport.CreateHandler<ServerVerificationUpdate>(HandleVerificationUpdate);

            Log.Dispose();

            Log = new LogOutput($"Central Client");
            Log.AddLogger(Logger.Instance);
            Log.AddLogger(LogOutput.Common.GetLogger<FileLogger>());

            Log.Info("Initialized!");

            updateTimer = new Timer(Update, null, 0, 15000);

            IsConnected = true;

            Timing.CallDelayed(3f, () => RequestVerification());
        }

        public override void OnStopped()
        {
            base.OnStopped();

            Transport.RemoveHandler<ServerVerificationUpdate>(HandleVerificationUpdate);

            updateTimer?.Dispose();
            updateTimer = null;

            IsConnected = false;

            Client = null;
            Requests = null;

            Log.Warn($"Disconnected from the central server! Attempting to reconnect ..");
        }

        public void HandleVerificationUpdate(ServerVerificationUpdate msg)
        {
            Log.Info($"Received a verification update message from the central server: {IsVerified} -> {msg.IsVerified}");
            IsVerified = msg.IsVerified;
            ServerStatic.PermissionsHandler._isVerified = msg.IsVerified;
        }

        private void Update(object _)
            => RequestUpdate();

        public static void ValidateAuthentification(string id, Action<AuthValidationResponse> callback = null)
        {
            if (!IsRequested || Client is null || Requests is null || !IsVerified)
                return;

            Client.Log.Info($"Requesting ID validation from the central server ..");

            Requests.Request<AuthValidationRequest, AuthValidationResponse>(new AuthValidationRequest(id, string.Empty), 0, (res, msg) =>
            {
                Client.Log.Info($"Received ID validation result of '{msg.Id}' from the central server ({msg.Result}; {msg.IsGlobalBan}; {msg.IsGlobalPerms}).");
                callback.Call(msg);
            });
        }

        public static void RequestUpdate()
        {
            if (!IsRequested || Client is null || Requests is null || !IsVerified)
                return;

            Requests.Request<ServerListUpdateRequest, ServerListUpdateResponse>(new ServerListUpdateRequest(Utils.GetList()), 0, (res, msg) =>
            {
                switch (msg.Result)
                {
                    case ServerListUpdateResult.Rejected:
                        Client.Log.Error($"Server list update was rejected by the central server due to an unknown reason!");
                        break;

                    case ServerListUpdateResult.NotVerified:
                        Client.Log.Error($"Server list update was rejected by the central server due to not being verified!");
                        break;
                }
            });
        }

        public static void RequestVerification(Action callback = null)
        {
            if (IsRequested || Client is null || Requests is null)
                return;

            Client.Log.Info($"Requesting server verification from the central server ..");

            Requests.Request<ServerVerificationRequest, ServerVerificationResponse>(new ServerVerificationRequest(Utils.GetList()), 0, (res, msg) =>
            {
                Client.Log.Info($"Received server verification response from the central server: {msg.IsVerified}");

                IsRequested = true;
                IsVerified = msg.IsVerified;

                if (IsVerified)
                    ServerStatic.PermissionsHandler.SetServerAsVerified();
                else
                    ServerStatic.PermissionsHandler._isVerified = false;

                callback.Call();
            });
        }
    }
}