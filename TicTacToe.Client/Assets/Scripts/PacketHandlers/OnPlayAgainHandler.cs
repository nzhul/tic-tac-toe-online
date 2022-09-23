using NetworkShared;
using NetworkShared.Attributes;
using System;

namespace TTT.PacketHandlers
{
    /// <summary>
    /// Play again request will be received only from the opponent!
    /// </summary>
    [HandlerRegister(PacketType.OnPlayAgain)]
    public class OnPlayAgainHandler : IPacketHandler
    {
        public static event Action OnPlayAgain;

        public void Handle(INetPacket packet, int connectionId)
        {
            OnPlayAgain?.Invoke();
        }
    }
}
