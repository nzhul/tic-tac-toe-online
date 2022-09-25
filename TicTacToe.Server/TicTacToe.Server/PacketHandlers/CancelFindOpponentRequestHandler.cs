using NetworkShared;
using NetworkShared.Attributes;
using TicTacToe.Server.Games;
using TicTacToe.Server.Matchmaking;

namespace TicTacToe.Server.PacketHandlers
{
    [HandlerRegister(PacketType.CancelFindOpponentRequest)]
    public class CancelFindOpponentRequestHandler : IPacketHandler
    {
        private readonly Matchmaker _matchmaker;
        private readonly UsersManager _usersManager;

        public CancelFindOpponentRequestHandler(Matchmaker matchmaker, UsersManager usersManager)
        {
            _matchmaker = matchmaker;
            _usersManager = usersManager;
        }

        public void Handle(INetPacket packet, int connectionId)
        {
            var connection = _usersManager.GetConnection(connectionId);
            _matchmaker.TryUnregisterPlayer(connection.User.Id);
        }
    }
}
