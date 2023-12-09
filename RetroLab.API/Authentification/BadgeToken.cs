using Network.Interfaces.Transporting;

using System.IO;

namespace RetroLab.API.Authentification
{
    public class BadgeToken : AuthToken
    {
        public string Name { get; set; }
        public string Color { get; set; }

        public ulong Permissions { get; set; }

        public BadgeToken() { }

        public override void Read(BinaryReader reader, ITransport transport)
        {
            base.Read(reader, transport);

            Name = reader.ReadString();
            Color = reader.ReadString();

            Permissions = reader.ReadUInt64();
        }

        public override void Write(BinaryWriter writer, ITransport transport)
        {
            base.Write(writer, transport);

            writer.Write(Name);
            writer.Write(Color);

            writer.Write(Permissions);
        }
    }
}