﻿using Network.Extensions;
using Network.Interfaces.Transporting;

using System.IO;

namespace RetroLab.API.Servers
{
    public struct ServerListUpdateRequest : IMessage
    {
        public ServerListInfo Info;

        public ServerListUpdateRequest(ServerListInfo info)
        {
            Info = info;
        }

        public void Read(BinaryReader reader, ITransport transport)
        {
            Info = reader.ReadObject<ServerListInfo>(transport);
        }

        public void Write(BinaryWriter writer, ITransport transport)
        {
            writer.WriteObject(Info, transport);
        }
    }
}