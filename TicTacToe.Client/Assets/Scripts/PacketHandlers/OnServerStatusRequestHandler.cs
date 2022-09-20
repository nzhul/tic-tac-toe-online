using NetworkShared;
using NetworkShared.Attributes;
using NetworkShared.Packets.ServerClient;
using System;

namespace Assets.Scripts.PacketHandlers
{
    [HandlerRegister(PacketType.OnServerStatus)]
    public class OnServerStatusRequestHandler : IPacketHandler
    {
        public static event Action<Net_OnServerStatus> OnServerStatus;

        public void Handle(INetPacket packet, int connectionId)
        {
            var msg = (Net_OnServerStatus)packet;
            OnServerStatus?.Invoke(msg);
        }
    }
}
