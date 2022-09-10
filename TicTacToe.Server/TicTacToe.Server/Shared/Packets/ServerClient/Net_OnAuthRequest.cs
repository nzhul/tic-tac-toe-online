using LiteNetLib.Utils;
using TicTacToe.Server.Shared;

namespace TicTacToe.Server.Packets.ServerClient
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
