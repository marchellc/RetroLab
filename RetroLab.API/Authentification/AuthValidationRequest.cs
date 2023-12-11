using Common.Extensions;

using Network.Interfaces.Transporting;

using System.IO;

namespace RetroLab.API.Authentification
{
    public struct AuthValidationRequest : IMessage
    {
        public string Id;
        public string Nick;

        public AuthValidationRequest(string id, string nick)
        {
            Id = id;
            Nick = nick;
        }

        public void Read(BinaryReader reader, ITransport transport)
        {
            Id = reader.ReadStringEx();
            Nick = reader.ReadStringEx();
        }

        public void Write(BinaryWriter writer, ITransport transport)
        {
            writer.WriteString(Id);
            writer.WriteString(Nick);
        }
    }
}