using Common.Logging;

using Network.Extensions;
using Network.Features;
using Network.Interfaces.Requests;

using RetroLab.API.Authentification;
using RetroLab.API.Players;
using RetroLab.API.Servers;

using RetroLab.Server.Authentification;
using RetroLab.Server.Servers;

namespace RetroLab.Server.Network
{
    public class NetworkPlayerHandler : Feature
    {
        public string Id { get; private set; }
        public string Name { get; private set; }

        public AuthToken Token { get; private set; }

        public IRequestManager Requests { get; private set; }

        public void Initialize(PlayerAuthResponse playerAuthResponse)
        {
            Id = playerAuthResponse.Id;
            Name = playerAuthResponse.Name;

            Token = AuthTokenLoader.GetPlayerToken(Id);

            Log.Dispose();

            Log = new LogOutput($"Player {Id}");
            Log.Setup();

            Log.Info($"Started the network handler. Generated token: {Token.Id}");

            Requests = Peer.Features.GetFeature<IRequestManager>();

            Requests.CreateHandler<ServerListDownloadRequest>(OnServerListDownloadRequest);
            Requests.CreateHandler<AuthTokenRequest>(OnPlayerAuthTokenRequest);

            Transport.Send(new AuthConfirmation());
        }

        private void OnPlayerAuthTokenRequest(IRequest request, AuthTokenRequest msg)
        {
            if (Token.IsExpired())
                Token = AuthTokenLoader.GetPlayerToken(Id);

            request.Success(new AuthTokenResponse(Token));
        }

        private void OnServerListDownloadRequest(IRequest request, ServerListDownloadRequest msg)
        {
            request.Success(new ServerListDownloadResponse(ServerListManager.GetVerified()));
        }
    }
}