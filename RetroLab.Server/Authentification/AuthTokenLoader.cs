using Common.IO.Collections;

using RetroLab.API.Authentification;

namespace RetroLab.Server.Authentification
{
    public static class AuthTokenLoader
    {
        private static readonly char[] IdCharacters;

        static AuthTokenLoader()
        {
            IdCharacters = "$%#@!*abcdefghijklmnopqrstuvwxyz1234567890?;:ABCDEFGHIJKLMNOPQRSTUVWXYZ^&".ToArray();
        }

        private static Timer timer;
        private static Random random;

        public static LockedList<AuthToken> Tokens { get; } = new LockedList<AuthToken>();

        public static int IdSize = 10;

        public static void Load()
        {
            timer = new Timer(OnUpdate, null, 0, 1000);
            Program.OnExiting += Exit;
        }

        public static void Exit()
        {
            Program.OnExiting -= Exit;

            timer.Dispose();
            timer = null;

            Tokens.Clear();
        }

        public static AuthToken FindToken(string id)
        {
            foreach (var token in Tokens)
            {
                if (token.Id == id)
                    return token;
            }

            return null;
        }

        public static AuthToken GetPlayerToken(string playerId)
        {
            foreach (var token in Tokens)
            {
                if (token.Target == playerId && !token.IsExpired() && token.Type is AuthTokenType.PlayerAuth)
                    return token;
            }

            var playerToken = new AuthToken
            {
                Generated = DateTime.Now,
                Expires = DateTime.Now.AddHours(1),

                Id = GetRandomId(),

                Target = playerId,

                Type = AuthTokenType.PlayerAuth
            };

            Tokens.Add(playerToken);

            return playerToken;
        }

        public static ServerToken GetServerToken(string ip, bool suppression = false, bool verification = true)
        {
            foreach (var token in Tokens)
            {
                if (token.Type is AuthTokenType.ServerAuth && token.Target == ip && !token.IsExpired())
                    return (ServerToken)token;
            }

            var serverToken = new ServerToken
            {
                Generated = DateTime.Now,
                Expires = DateTime.Now.AddHours(1),

                Id = GetRandomId(),

                IsSuppressed = suppression,
                IsVerified = verification,

                Target = ip,

                Type = AuthTokenType.ServerAuth
            };

            Tokens.Add(serverToken);

            return serverToken;
        }

        public static string GetRandomId()
        {
            var str = "";

            for (int i = 0; i < IdSize; i++)
                str += IdCharacters[random.Next(0, IdCharacters.Length)];

            return str;
        }

        private static void OnUpdate(object _)
            => Tokens.RemoveRange(t => t.IsExpired());
    }
}