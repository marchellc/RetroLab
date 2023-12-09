using Common.Extensions;

using Network.Extensions;
using Network.Interfaces.Transporting;

using System.Collections.Generic;
using System.IO;

namespace RetroLab.API.Servers
{
    public struct ServerListDownloadResponse : IMessage
    {
        public List<ServerInfo> Servers;

        public ServerListDownloadResponse(List<ServerInfo> servers)
        {
            Servers = servers;
        }

        public void Read(BinaryReader reader, ITransport transport)
        {
            Servers = reader.ReadList(() => reader.ReadObject<ServerInfo>(transport));
        }

        public void Write(BinaryWriter writer, ITransport transport)
        {
            writer.WriteItems(Servers, s => writer.WriteObject(s, transport));
        }
    }
}