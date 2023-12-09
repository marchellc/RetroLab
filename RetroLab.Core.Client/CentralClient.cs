using Common.Logging;
using Common.Reflection;

using Network.Extensions;
using Network.Features;
using Network.Requests;

using RetroLab.API.Authentification;
using RetroLab.API.Players;
using RetroLab.API.Servers;

using System;
using System.Collections.Generic;
using System.Threading;

namespace RetroLab
{
    public class CentralClient : Feature
    {
        public static CentralClient Client;
        public static RequestManager Requests;
        public static AuthToken Token;

        public static List<API.Servers.ServerInfo> Servers;

        private Timer timer;

        private bool tokenRequested;
        private bool listRequested;

        public override void OnStarted()
        {
            base.OnStarted();

            Client = this;

            Requests = Peer.Features.GetFeature<RequestManager>();

            Transport.CreateHandler<AuthConfirmation>(OnAuthConfirmation);
            Transport.CreateHandler<AuthRequest>(OnAuthRequest);

            timer = new Timer(UpdateToken, null, 0, 1000);

            Servers = new List<API.Servers.ServerInfo>();

            Log.Dispose();

            Log = new LogOutput($"Central Client");
            Log.Setup();

            Log.Info("Initialized!");
        }

        public override void OnStopped()
        {
            base.OnStopped();

            Servers.Clear();
            Servers = null;

            timer.Dispose();
            timer = null;

            Client = null;
            Requests = null;
            Token = null;
        }

        public void RequestToken()
        {
            if (tokenRequested)
                return;

            tokenRequested = true;

            Log.Info("Requesting a new auth token from the central server ..");

            Requests.Request<AuthTokenRequest, AuthTokenResponse>(new AuthTokenRequest(DiscordClient.Id, string.Empty), 0, (res, msg) =>
            {
                Token = msg.Token;
                Log.Info($"Received a new auth token from the central server: {msg.Token.Id}");
                tokenRequested = false;
            });
        }

        public void RequestList(Action<List<API.Servers.ServerInfo>> callback = null)
        {
            if (listRequested)
                return;

            listRequested = true;

            Log.Info($"Requesting a new server list from the central server ..");

            Requests.Request<ServerListDownloadRequest, ServerListDownloadResponse>(new ServerListDownloadRequest(), 0, (res, msg) =>
            {
                Servers.Clear();
                Servers.AddRange(msg.Servers);

                callback.Call(Servers);

                Log.Info($"Received {Servers.Count} server(s) from the central server.");

                listRequested = false;
            });
        }

        private void OnAuthRequest(AuthRequest msg)
        {
            Log.Info("Authentification requested.");
            Transport.Send(new PlayerAuthResponse(DiscordClient.Id, DiscordClient.Name));
            Log.Info("Authentification sent.");
        }

        private void OnAuthConfirmation(AuthConfirmation confirmation)
        {
            Log.Info("Authentification confirmed.");
            RequestToken();
        }

        private void UpdateToken(object _)
        {
            if (Token is null)
                return;

            if (!tokenRequested && Token.IsExpired())
                RequestToken();
        }
    }
}