using Network.Interfaces.Transporting;

using System.IO;

namespace RetroLab.API.Authentification
{
    public struct AuthTokenRequest : IMessage
    {
        public string Id;
        public string Ip;

        public AuthTokenRequest(string id, string ip)
        {
            Id = id;
            Ip = ip;
        }

        public void Read(BinaryReader reader, ITransport transport)
        {
            Id = reader.ReadString();
            Ip = reader.ReadString();
        }

        public void Write(BinaryWriter writer, ITransport transport)
        {
            writer.Write(Id);
            writer.Write(Ip);
        }
    }
}