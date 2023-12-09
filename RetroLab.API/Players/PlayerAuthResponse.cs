using Common.Extensions;

using Network.Interfaces.Transporting;

using System.IO;

namespace RetroLab.API.Players
{
    public struct PlayerAuthResponse : IMessage
    {
        public string Id;
        public string Name;

        public PlayerAuthResponse(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public void Read(BinaryReader reader, ITransport transport)
        {
            Id = reader.ReadStringEx();
            Name = reader.ReadStringEx();
        }

        public void Write(BinaryWriter writer, ITransport transport)
        {
            writer.WriteString(Id);
            writer.WriteString(Name);
        }
    }
}