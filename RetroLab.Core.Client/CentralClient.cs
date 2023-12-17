using Common.IO.Collections;
using Common.Logging;
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

        public static readonly LockedList<ServerListInfo> Servers = new LockedList<ServerListInfo>();

        public static bool IsListRequested;
        public static bool IsConnected;
        public static bool IsAuthed;

        public static AuthValidationResponse AuthResponse;

        private Timer timer;

        public override void OnStarted()
        {
            base.OnStarted();

            Client = this;

            Requests = Peer.Features.GetFeature<RequestManager>();

            Log.Dispose();

            Log = new LogOutput($"Central Client");
            Log.AddLogger(Logger.Instance);

            Log.Info("Initialized!");

            IsConnected = true;

            Timing.CallDelayed(1f, () => DownloadAuth());
        }

        public override void OnStopped()
        {
            Utils.Disconnect("Disconnected from the central server.");

            timer?.Dispose();
            timer = null;

            IsConnected = false;
            IsListRequested = false;
            IsAuthed = false;

            base.OnStopped();

            Servers.Clear();

            Client = null;
            Requests = null;

            AuthResponse = default;

            Log.Warn($"The central server client has disconnected!");
        }

        public static void DownloadAuth(Action<AuthValidationResponse> callback = null)
        {
            if (IsAuthed || !IsConnected || Client is null || Requests is null)
                return;

            Client.Log.Info($"Requesting authentification from the central server ..");

            Requests.Request<AuthValidationRequest, AuthValidationResponse>(new AuthValidationRequest(DiscordClient.Id, DiscordClient.Name), 0, (res, msg) =>
            {
                IsAuthed = true;

                AuthResponse = msg;

                Client.Log.Info($"Received an auth response: {msg.Id} ({msg.Result})");

                callback.Call(msg);

                if (Client.timer is null)
                    Client.timer = new Timer(_ =>
                    {
                        Timing.CallDelayed(0.1f, ServerListManager.singleton.Refresh);
                    }, null, 150, 15000);
            });
        }

        public static void DownloadServerList(Action<LockedList<ServerListInfo>> callback = null)
        {
            if (IsListRequested || !IsConnected || Client is null || Requests is null)
                return;

            IsListRequested = true;

            Client.Log.Info($"Requesting a new server list from the central server ..");

            try
            {
                Requests.Request<ServerListDownloadRequest, ServerListDownloadResponse>(new ServerListDownloadRequest(DiscordClient.Id), 0, (res, msg) =>
                {
                    try
                    {
                        IsListRequested = false;

                        Servers.Clear();

                        callback?.Invoke(Servers);

                        if (msg.Servers is null)
                        {
                            Client.Log.Error($"The received server array is null!");
                            return;
                        }

                        if (msg.Servers.Length <= 0)
                        {
                            Client.Log.Warn($"Received an empty server list.");
                            return;
                        }

                        Client.Log.Info($"Received {msg.Servers.Length} server(s) from the central server.");
                        Servers.AddRange(msg.Servers);
                    }
                    catch (Exception ex)
                    {
                        Client.Log.Error(ex);
                    }
                });
            }
            catch (Exception ex)
            {
                Client.Log.Error(ex);
            }
        }
    }
}