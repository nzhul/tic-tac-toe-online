using Assets.Scripts.Packets;

namespace Assets.Scripts.PacketHandlers
{
    public interface IPacketHandler
    {
        void Handle(INetPacket packet, int connectionId);
    }
}
