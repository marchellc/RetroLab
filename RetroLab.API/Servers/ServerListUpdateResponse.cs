using Network.Interfaces.Transporting;

using System.IO;

namespace RetroLab.API.Servers
{
    public struct ServerListUpdateResponse : IMessage
    {
        public ServerListUpdateResult Result;

        public ServerListUpdateResponse(ServerListUpdateResult result)
        {
            Result = result;
        }

        public void Read(BinaryReader reader, ITransport transport)
        {
            Result = (ServerListUpdateResult)reader.ReadByte();
        }

        public void Write(BinaryWriter writer, ITransport transport)
        {
            writer.Write((byte)Result);
        }
    }
}
