using LiteNetLib.Utils;

namespace NetworkShared
{
    public enum PacketType : byte
    {
        #region ClientServer
        Invalid = 0,
        AuthRequest = 1,
        ServerStatusRequest = 2,
        FindOpponentRequest = 3,
        CancelFindOpponentRequest = 4,
        SurrenderRequest = 5,
        QuitGameRequest = 6,
        PlayAgainRequest = 7,
        AcceptPlayAgainRequest = 8,
        MarkCellRequest = 9,
        #endregion

        #region ServerClient
        OnAuth = 100,
        OnAuthFail = 101,
        OnServerStatus = 102,
        OnFindOpponent = 103,
        OnStartGame = 104,
        OnSurrender = 105,
        OnQuitGame = 106,
        OnPlayAgain = 107,
        OnNewRound = 108,
        OnMarkCell = 109
        #endregion
    }

    public interface INetPacket : INetSerializable
    {
        PacketType Type { get; }
    }
}
