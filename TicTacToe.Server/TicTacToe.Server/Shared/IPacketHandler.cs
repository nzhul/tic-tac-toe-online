using TicTacToe.Server.Packets;

namespace TicTacToe.Server.Shared
{
    public interface IPacketHandler
    {
        void Handle(INetPacket packet, int connectionId);
    }
}
