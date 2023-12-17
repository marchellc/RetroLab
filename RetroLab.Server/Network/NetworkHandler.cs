using Common.IO.Collections;

using Network.Extensions;
using Network.Features;
using Network.Interfaces.Requests;
using Network.Requests;

using RetroLab.API.Authentification;
using RetroLab.API.Servers;

namespace RetroLab.Server.Network
{
    public class NetworkHandler : Feature
    {
        public static LockedList<NetworkHandler> Handlers { get; } = new LockedList<NetworkHandler>();
        public static LockedDictionary<string, AuthValidationRequest> AuthCache { get; } = new LockedDictionary<string, AuthValidationRequest>();

        public string Id;
        public string Ip;

        public int Port;

        public bool IsVerified;
        public bool IsServer;
        public bool IsLoaded;

        public ServerListInfo? List;

        public RequestManager Requests;

        public override void OnStarted()
        {
            base.OnStarted();

            Requests = Peer.Features.GetFeature<RequestManager>();

            Requests.CreateHandler<ServerListDownloadRequest>(OnServerListDownloadRequest);
            Requests.CreateHandler<ServerListUpdateRequest>(OnServerListUpdateRequest);
            Requests.CreateHandler<ServerVerificationRequest>(OnServerVerificationRequest);

            Requests.CreateHandler<AuthValidationRequest>(OnAuthValidationRequest);

            Handlers.Add(this);

            Log.Info("Initialized.");
        }

        public override void OnStopped()
        {
            base.OnStopped();

            IsVerified = false;
            IsServer = false;
            IsLoaded = false;

            Id = null;
            Ip = null;

            Port = 0;

            List = null;

            Requests = null;

            Handlers.Remove(this);
        }

        public void SendVerificationUpdate(bool status)
        {
            if (!IsLoaded)
            {
                Log.Warn($"Attempted to send a verification update message to a non-authorized handler.");
                return;
            }

            if (!IsServer)
            {
                Log.Warn($"Attempted to send a verification update message to a client authority.");
                return;
            }

            IsVerified = status;

            Transport.Send(new ServerVerificationUpdate(IsVerified));

            Log.Info($"Send a verification update message (new status: {status})");
        }

        private void OnAuthValidationRequest(IRequest request, AuthValidationRequest msg)
        {
            Log.Debug($"Received an auth validation request for ID: {msg.Id}");

            if (string.IsNullOrWhiteSpace(msg.Id))
            {
                Log.Warn($"Request contains an empty ID, rejecting as Invalid.");
                request.Success(new AuthValidationResponse(msg.Id, string.Empty, false, false, AuthValidationResult.MissingId));
                return;
            }

            if (msg.Id.Length != 18)
            {
                Log.Warn($"Request contains an invalid ID; unexpected ID length ({msg.Id.Length}, needs to be 18), rejecting as Invalid.");
                request.Success(new AuthValidationResponse(msg.Id, string.Empty, false, false, AuthValidationResult.InvalidId));
                return;
            }

            if (IsServer)
            {
                if (!AuthCache.TryGetValue(msg.Id, out var cache) || string.IsNullOrWhiteSpace(cache.Nick))
                {
                    Log.Warn($"Request contains no such client request; rejecting as Invalid.");
                    request.Success(new AuthValidationResponse(msg.Id, string.Empty, false, false, AuthValidationResult.InvalidId));
                    return;
                }

                var isBanned = NetworkBanManager.IsBanned(msg.Id);
                var isMod = NetworkRoleManager.IsModerator(msg.Id);

                Log.Info($"Responding with Ok result ({isBanned}, {isMod})");

                request.Success(new AuthValidationResponse(msg.Id, cache.Nick, isMod, isBanned, AuthValidationResult.Ok));
            }
            else
            {
                AuthCache[msg.Id] = msg;

                Log.Debug($"Cached {msg.Id} request with nick {msg.Nick}");

                var isBanned = NetworkBanManager.IsBanned(msg.Id);
                var isMod = NetworkRoleManager.IsModerator(msg.Id);

                Log.Debug($"Responding with Ok result ({isBanned}, {isMod})");

                request.Success(new AuthValidationResponse(msg.Id, msg.Nick, isMod, isBanned, AuthValidationResult.Ok));
            }
        }

        private void OnServerListDownloadRequest(IRequest request, ServerListDownloadRequest msg)
        {
            Log.Debug($"Received a server list download request.");

            if (!IsLoaded)
                SetClientAuthority(msg.Id);

            if (IsServer)
            {
                Log.Warn($"Request received from a server authority, rejecting.");
                request.Fail(new ServerListDownloadResponse(null));
                return;
            }

            Log.Debug($"Responding with a list of verified servers.");
            request.Success(new ServerListDownloadResponse(NetworkListManager.GetServers()));
        }

        private void OnServerListUpdateRequest(IRequest request, ServerListUpdateRequest msg)
        {
            if (!IsLoaded)
                SetServerAuthority(msg.Info);

            if (!IsServer)
            {
                Log.Warn($"Request received from a client authority, rejecting.");
                request.Fail(new ServerListUpdateResponse(ServerListUpdateResult.Rejected));
                return;
            }

            if (!IsVerified)
            {
                Log.Warn($"Request received from a non-verified server, rejecting.");
                request.Fail(new ServerListUpdateResponse(ServerListUpdateResult.NotVerified));
                return;
            }

            if (string.IsNullOrWhiteSpace(msg.Info.Name) || string.IsNullOrWhiteSpace(msg.Info.Pastebin) 
                || string.IsNullOrWhiteSpace(msg.Info.Ip) || msg.Info.Port <= 0 || msg.Info.Port >= (short.MaxValue * 2)
                || msg.Info.MaxPlayers <= 0 || msg.Info.Players < 0)
            {
                Log.Warn($"Request contains invalid data, rejecting.");
                request.Fail(new ServerListUpdateResponse(ServerListUpdateResult.Rejected));
                return;
            }

            List = msg.Info;

            request.Success(new ServerListUpdateResponse(ServerListUpdateResult.Ok));
        }

        private void OnServerVerificationRequest(IRequest request, ServerVerificationRequest msg)
        {
            if (!IsLoaded)
                SetServerAuthority(msg.Info);

            if (!IsServer)
            {
                Log.Warn($"Request received from a client authority, rejecting.");
                request.Fail(new ServerVerificationResponse(false));
                return;
            }

            request.Success(new ServerVerificationResponse(IsVerified));
        }

        private void SetServerAuthority(ServerListInfo info)
        {
            Id = null;

            IsServer = true;
            IsLoaded = true;

            List = info;

            Ip = info.Ip;
            Port = info.Port;

            IsVerified = NetworkListManager.IsVerified(Ip);

            Log.Info($"Set handler authority to SERVER ({Ip}:{Port}): {IsVerified}");
        }

        private void SetClientAuthority(string id)
        {
            IsServer = false;
            IsVerified = false;
            IsLoaded = true;

            Id = id;

            List = null;

            Log.Info($"Set handler authority to CLIENT ({id})");
        }
    }
}
