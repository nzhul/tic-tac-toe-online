using Assets.Scripts.Attributes;
using Assets.Scripts.Packets;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.PacketHandlers
{
    [HandlerRegister(PacketType.OnAuthRequest)]
    public class OnAuthRequestHandler : IPacketHandler
    {
        public void Handle(INetPacket packet, int connectionId)
        {
            Debug.Log("Received OnAuthRequest message from the server!");
            SceneManager.LoadScene("01_Lobby");
        }
    }
}
