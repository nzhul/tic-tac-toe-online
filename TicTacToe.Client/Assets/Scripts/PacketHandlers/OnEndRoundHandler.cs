using NetworkShared;
using NetworkShared.Attributes;
using NetworkShared.Packets.ServerClient;
using System;

namespace TTT.PacketHandlers
{
    [HandlerRegister(PacketType.OnEndRound)]
    public class OnEndRoundHandler : IPacketHandler
    {
        public static event Action<Net_OnEndRound> OnEndRound;

        public void Handle(INetPacket packet, int connectionId)
        {
            var msg = (Net_OnEndRound)packet;
            OnEndRound?.Invoke(msg);
        }
    }
}
