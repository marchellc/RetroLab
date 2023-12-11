using Network.Interfaces.Transporting;

using System.IO;

namespace RetroLab.API.Servers
{
    public struct ServerVerificationResponse : IMessage
    {
        public bool IsVerified;

        public ServerVerificationResponse(bool isVerified)
        {
            IsVerified = isVerified;
        }

        public void Read(BinaryReader reader, ITransport transport)
        {
            IsVerified = reader.ReadBoolean();
        }

        public void Write(BinaryWriter writer, ITransport transport)
        {
            writer.Write(IsVerified);
        }
    }
}