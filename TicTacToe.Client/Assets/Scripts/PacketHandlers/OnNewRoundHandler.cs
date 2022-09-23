using NetworkShared;
using NetworkShared.Attributes;
using System;

namespace Assets.Scripts.PacketHandlers
{
    [HandlerRegister(PacketType.OnNewRound)]
    public class OnNewRoundHandler : IPacketHandler
    {
        public static event Action OnNewRound;

        public void Handle(INetPacket packet, int connectionId)
        {
            OnNewRound?.Invoke();
        }
    }
}
