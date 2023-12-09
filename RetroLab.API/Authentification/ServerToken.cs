using Network.Interfaces.Transporting;

using System.IO;

namespace RetroLab.API.Authentification
{
    public class ServerToken : AuthToken
    {
        public bool IsSuppressed { get; set; }
        public bool IsVerified { get; set; }

        public ServerToken() { }

        public override void Read(BinaryReader reader, ITransport transport)
        {
            base.Read(reader, transport);

            IsSuppressed = reader.ReadBoolean();
            IsVerified = reader.ReadBoolean();
        }

        public override void Write(BinaryWriter writer, ITransport transport)
        {
            base.Write(writer, transport);

            writer.Write(IsSuppressed);
            writer.Write(IsVerified);
        }

        public bool IsServerList()
            => IsVerified && !IsSuppressed && !IsExpired();
    }
}
