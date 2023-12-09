using Network.Extensions;
using Network.Interfaces.Transporting;

using System.IO;

namespace RetroLab.API.Servers
{
    public class ServerInfo : IMessage
    {
        public string Ip { get;set; }

        public int Port { get; set; }

        public ServerListInfo Info;

        public ServerInfo() { }

        public void Read(BinaryReader reader, ITransport transport)
        {
            Ip = reader.ReadString();

            Port = reader.ReadInt32();

            Info = reader.ReadObject<ServerListInfo>(transport);
        }

        public void Write(BinaryWriter writer, ITransport transport)
        {
            writer.Write(Ip);

            writer.Write(Port);

            writer.WriteObject(Info, transport);
        }
    }
}