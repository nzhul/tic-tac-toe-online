using LiteNetLib.Utils;

namespace Assets.Scripts.Packets
{
    public enum PacketType : byte
    {
        #region ClientServer
        Invalid = 0,
        AuthRequest = 1,
        #endregion

        #region ServerClient
        OnAuthRequest = 100
        #endregion
    }

    public interface INetPacket : INetSerializable
    {
        PacketType Type { get; }
    }
}
