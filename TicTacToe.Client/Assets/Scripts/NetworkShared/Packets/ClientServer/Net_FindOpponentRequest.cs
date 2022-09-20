using LiteNetLib.Utils;
using NetworkShared;

namespace NetworkShared.Packets.ClientServer
{
    public struct Net_FindOpponentRequest : INetPacket
    {
        public PacketType Type => PacketType.FindOpponentRequest;

        public void Deserialize(NetDataReader reader)
        {
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put((byte)Type);
        }
    }
}
