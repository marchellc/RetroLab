using Common.Extensions;

using Network.Interfaces.Transporting;

using System.IO;

namespace RetroLab.API.Servers
{
    public struct ServerListDownloadRequest : IMessage
    {
        public string Id;

        public ServerListDownloadRequest(string id)
        {
            Id = id;    
        }

        public void Read(BinaryReader reader, ITransport transport)
        {
            Id = reader.ReadStringEx();
        }

        public void Write(BinaryWriter writer, ITransport transport) 
        {
            writer.WriteString(Id);
        }
    }
}