using Network.Interfaces.Transporting;

using System.IO;

namespace RetroLab.API.Authentification
{
    public struct AuthTokenValidationRequest : IMessage
    {
        public string TokenId;

        public string UserId;
        public string UserIp;

        public AuthTokenType Type;

        public AuthTokenValidationRequest(string id, string uid, string uip, AuthTokenType type)
        {
            TokenId = id;

            UserId = uid;
            UserIp = uip;

            Type = type;
        }

        public void Read(BinaryReader reader, ITransport transport)
        {
            TokenId = reader.ReadString();

            UserId = reader.ReadString();
            UserIp = reader.ReadString();

            Type = (AuthTokenType)reader.ReadByte();
        }

        public void Write(BinaryWriter writer, ITransport transport)
        {
            writer.Write(TokenId);

            writer.Write(UserId);
            writer.Write(UserIp);

            writer.Write((byte)Type);
        }
    }
}