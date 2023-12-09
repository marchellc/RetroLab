using Network.Extensions;
using Network.Interfaces.Transporting;

using System.IO;

namespace RetroLab.API.Authentification
{
    public struct AuthTokenResponse : IMessage
    {
        public AuthToken Token;

        public AuthTokenResponse(AuthToken token) { Token = token; }

        public void Read(BinaryReader reader, ITransport transport)
        {
            Token = reader.ReadObject<AuthToken>(transport);
        }

        public void Write(BinaryWriter writer, ITransport transport)
        {
            writer.WriteObject(Token, transport);
        }
    }
}