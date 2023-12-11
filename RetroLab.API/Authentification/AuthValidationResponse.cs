using Network.Interfaces.Transporting;

using System.IO;

namespace RetroLab.API.Authentification
{
    public struct AuthValidationResponse : IMessage
    {
        public string Id;
        public string Nick;

        public bool IsGlobalPerms;
        public bool IsGlobalBan;

        public AuthValidationResult Result;

        public AuthValidationResponse(string id, string nick, bool globalPerms, bool globalBan, AuthValidationResult result)
        {
            Id = id;
            Nick = nick;

            IsGlobalPerms = globalPerms;
            IsGlobalBan = globalBan;

            Result = result;
        }

        public void Read(BinaryReader reader, ITransport transport)
        {
            Id = reader.ReadString();
            Nick = reader.ReadString();

            IsGlobalBan = reader.ReadBoolean();
            IsGlobalPerms = reader.ReadBoolean();

            Result = (AuthValidationResult)reader.ReadByte();
        }

        public void Write(BinaryWriter writer, ITransport transport)
        {
            writer.Write(Id);
            writer.Write(Nick);

            writer.Write(IsGlobalBan);
            writer.Write(IsGlobalPerms);

            writer.Write((byte)Result);
        }
    }
}
