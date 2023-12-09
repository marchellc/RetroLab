using Common.Extensions;

using Network.Interfaces.Transporting;

using System;
using System.IO;

namespace RetroLab.API.Authentification
{
    public class AuthToken : IMessage
    {
        public string Id { get; set; }
        public string Target { get; set; }

        public DateTime Generated { get; set; }
        public DateTime Expires { get; set; }

        public AuthTokenType Type { get; set; }

        public AuthToken() { }

        public virtual void Read(BinaryReader reader, ITransport transport)
        {
            Id = reader.ReadString();
            Target = reader.ReadString();

            Generated = reader.ReadDate();
            Expires = reader.ReadDate();

            Type = (AuthTokenType)reader.ReadByte();
        }

        public virtual void Write(BinaryWriter writer, ITransport transport)
        {
            writer.Write(Id);
            writer.Write(Target);

            writer.WriteDate(Generated);
            writer.WriteDate(Expires);

            writer.Write((byte)Type);
        }

        public bool IsExpired()
            => DateTime.Now >= Expires;
    }
}