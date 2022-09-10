using LiteNetLib.Utils;

namespace Assets.Scripts.Packets.ServerClient
{
    public struct Net_OnAuthRequest : INetPacket
    {
        public PacketType Type => PacketType.OnAuthRequest;

        public void Deserialize(NetDataReader reader)
        {
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put((byte)Type);
        }
    }
}
