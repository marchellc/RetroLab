using Network.Interfaces.Transporting;

using System.IO;

namespace RetroLab.API.Servers
{
    public struct ServerListInfo : IMessage
    {
        public string Name;
        public string Pastebin;

        public int Players;
        public int MaxPlayers;

        public ServerListInfo(string name, string pastebin, int players, int maxPlayers)
        {
            Name = name;
            Pastebin = pastebin;

            Players = players;
            MaxPlayers = maxPlayers;
        }

        public void Read(BinaryReader reader, ITransport transport)
        {
            Name = reader.ReadString();
            Pastebin = reader.ReadString();

            Players = reader.ReadInt32();
            MaxPlayers = reader.ReadInt32();
        }

        public void Write(BinaryWriter writer, ITransport transport)
        {
            writer.Write(Name);
            writer.Write(Pastebin);

            writer.Write(Players);
            writer.Write(MaxPlayers);
        }
    }
}