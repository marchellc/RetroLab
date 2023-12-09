using Common.Extensions;

using Network.Extensions;
using Network.Interfaces.Transporting;

using System.IO;

namespace RetroLab.API.Servers
{
    public struct ServerAuthResponse : IMessage
    {
        public string Ip;

        public int Port;

        public ServerListInfo Info;

        public ServerAuthResponse(string ip, int port, ServerListInfo serverListInfo)
        {
            Ip = ip;
            Port = port;
            Info = serverListInfo;
        }

        public void Read(BinaryReader reader, ITransport transport)
        {
            Ip = reader.ReadStringEx();
            Port = reader.ReadInt32();
            Info = reader.ReadObject<ServerListInfo>(transport);
        }

        public void Write(BinaryWriter writer, ITransport transport)
        {
            writer.WriteString(Ip);
            writer.Write(Port);
            writer.WriteObject(Info, transport);
        }
    }
}
