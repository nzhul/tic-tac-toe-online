using LiteNetLib.Utils;

namespace NetworkShared.Packets.ClientServer
{
    public struct Net_AcceptPlayAgainRequest : INetPacket
    {
        public PacketType Type => PacketType.AcceptPlayAgainRequest;

        public void Deserialize(NetDataReader reader)
        {
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put((byte)Type);
        }
    }
}
