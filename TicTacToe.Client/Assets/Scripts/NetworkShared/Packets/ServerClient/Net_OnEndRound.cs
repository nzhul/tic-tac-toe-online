using LiteNetLib.Utils;

namespace NetworkShared.Packets.ServerClient
{
    public struct Net_OnEndRound : INetPacket
    {
        public PacketType Type => PacketType.OnEndRound;

        public string Winner { get; set; }

        public void Deserialize(NetDataReader reader)
        {
            Winner = reader.GetString();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put((byte)Type);
            writer.Put(Winner);
        }
    }
}
