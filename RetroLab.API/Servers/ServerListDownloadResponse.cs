using Common.Extensions;

using Network.Extensions;
using Network.Interfaces.Transporting;

using System.IO;

namespace RetroLab.API.Servers
{
    public struct ServerListDownloadResponse : IMessage
    {
        public ServerListInfo[] Servers;

        public ServerListDownloadResponse(ServerListInfo[] servers)
        {
            Servers = servers;
        }

        public void Read(BinaryReader reader, ITransport transport)
        {
            Servers = reader.ReadArray(false, () => reader.ReadObject<ServerListInfo>(transport));
        }

        public void Write(BinaryWriter writer, ITransport transport)
        {
            writer.WriteItems(Servers, s => writer.WriteObject(s, transport));
        }
    }
}