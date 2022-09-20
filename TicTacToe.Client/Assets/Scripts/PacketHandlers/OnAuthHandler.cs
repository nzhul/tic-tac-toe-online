using NetworkShared;
using NetworkShared.Attributes;
using NetworkShared.Shared;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TTT.PacketHandlers
{
    [HandlerRegister(PacketType.OnAuth)]
    public class OnAuthHandler : IPacketHandler
    {
        public void Handle(INetPacket packet, int connectionId)
        {
            Debug.Log("Received OnAuthRequest message from the server!");
            SceneManager.LoadScene("01_Lobby");
        }
    }
}
