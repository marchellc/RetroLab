using Common.Logging;

using Network.Extensions;
using Network.Features;
using Network.Interfaces.Requests;

using RetroLab.API.Authentification;
using RetroLab.API.Servers;

using RetroLab.Server.Authentification;
using RetroLab.Server.Servers;

namespace RetroLab.Server.Network
{
    public class NetworkServerHandler : Feature
    {
        public string Ip { get; private set; }

        public int Port { get; private set; }

        public ServerToken Token { get; private set; }

        public IRequestManager Requests { get; private set; }

        public void Initialize(ServerAuthResponse serverAuthResponse)
        {
            Ip = serverAuthResponse.Ip;

            Port = serverAuthResponse.Port;

            Token = AuthTokenLoader.GetServerToken(Ip, false, ServerListManager.IsVerified(Ip));

            Log.Dispose();

            Log = new LogOutput($"Server {Ip}:{Port}");
            Log.Setup();

            Log.Info($"Server connected. Generated auth token: {Token.Id}");

            if (Token.IsServerList())
                ServerListManager.AddServer(Ip, Port, serverAuthResponse.Info);

            Requests = Peer.Features.GetFeature<IRequestManager>();

            Requests.CreateHandler<AuthTokenRequest>(OnServerAuthTokenRequest);
            Requests.CreateHandler<AuthTokenValidationRequest>(OnServerAuthTokenValidationRequest);
            Requests.CreateHandler<ServerListUpdateRequest>(OnServerListUpdateRequest);

            Transport.Send(new AuthConfirmation());
        }

        public override void OnStopped()
        {
            base.OnStopped();

            ServerListManager.RemoveServer(Ip, Port);

            Token = null;
            Ip = null;

            Port = 0;
        }

        private void OnServerAuthTokenRequest(IRequest request, AuthTokenRequest msg)
        {
            if (Token.IsExpired())
                Token = AuthTokenLoader.GetServerToken(Ip, Token.IsSuppressed, Token.IsVerified);

            request.Success(new AuthTokenResponse(Token));
        }

        private void OnServerAuthTokenValidationRequest(IRequest request, AuthTokenValidationRequest msg)
        {
            var token = AuthTokenLoader.FindToken(msg.TokenId);

            if (token is null)
            {
                request.Success(new AuthTokenValidationResponse(token, false, AuthTokenValidationResult.Invalid));
                return;
            }

            if (token.IsExpired())
            {
                request.Success(new AuthTokenValidationResponse(token, false, AuthTokenValidationResult.Expired));
                return;
            }

            if (token.Target != msg.UserId)
            {
                request.Success(new AuthTokenValidationResponse(token, false, AuthTokenValidationResult.MismatchedID));
                return;
            }

            if (token.Type != msg.Type)
            {
                request.Success(new AuthTokenValidationResponse(token, false, AuthTokenValidationResult.MismatchedUsage));
                return;
            }

            request.Success(new AuthTokenValidationResponse(token, false, AuthTokenValidationResult.Ok));
        }

        private void OnServerListUpdateRequest(IRequest request, ServerListUpdateRequest msg)
        {
            if (!Token.IsServerList())
            {
                request.Success(new ServerListUpdateResponse(ServerListUpdateResult.NotVerified));
                return;
            }

            request.Success(new ServerListUpdateResponse(ServerListManager.ProcessUpdate(Ip, Port, msg)));
        }
    }
}