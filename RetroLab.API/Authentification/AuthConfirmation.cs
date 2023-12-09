using Network.Interfaces.Transporting;

using System.IO;

namespace RetroLab.API.Authentification
{
    public struct AuthConfirmation : IMessage
    {
        public void Read(BinaryReader reader, ITransport transport) { }
        public void Write(BinaryWriter writer, ITransport transport) { }
    }
}