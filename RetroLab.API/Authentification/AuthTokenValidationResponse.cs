using Network.Extensions;
using Network.Interfaces.Transporting;

using System.IO;

namespace RetroLab.API.Authentification
{
    public struct AuthTokenValidationResponse : IMessage
    {
        public AuthToken Token;
        public AuthTokenValidationResult Result;
        public bool IsBanned;

        public AuthTokenValidationResponse(AuthToken token, bool isBanned, AuthTokenValidationResult result)
        {
            Token = token;
            Result = result;
            IsBanned = isBanned;
        }

        public void Read(BinaryReader reader, ITransport transport)
        {
            Token = reader.ReadObject<AuthToken>(transport);
            Result = (AuthTokenValidationResult)reader.ReadByte();
            IsBanned = reader.ReadBoolean();
        }

        public void Write(BinaryWriter writer, ITransport transport)
        {
            writer.WriteObject(Token, transport);
            writer.Write((byte)Result);
            writer.Write(IsBanned);
        }
    }
}
