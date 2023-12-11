using Network.Interfaces.Transporting;

using System.IO;

namespace RetroLab.API.Servers
{
    public struct ServerListInfo : IMessage
    {
        public string Name;
        public string Pastebin;
        public string Ip;

        public int Players;
        public int MaxPlayers;
        public int Port;

        public ServerListInfo(string name, string pastebin, string ip, int players, int maxPlayers, int port)
        {
            Name = name;
            Pastebin = pastebin;
            Ip = ip;

            Players = players;
            MaxPlayers = maxPlayers;
            Port = port;
        }

        public void Read(BinaryReader reader, ITransport transport)
        {
            Name = reader.ReadString();
            Pastebin = reader.ReadString();
            Ip = reader.ReadString();

            Players = reader.ReadInt32();
            MaxPlayers = reader.ReadInt32();
            Port = reader.ReadInt32();
        }

        public void Write(BinaryWriter writer, ITransport transport)
        {
            writer.Write(Name);
            writer.Write(Pastebin);
            writer.Write(Ip);

            writer.Write(Players);
            writer.Write(MaxPlayers);
            writer.Write(Port);
        }
    }
}