using NetworkShared;
using NetworkShared.Attributes;
using TicTacToe.Server.Game;
using TicTacToe.Server.Matchmaking;

namespace TicTacToe.Server.PacketHandlers
{
    [HandlerRegister(PacketType.FindOpponentRequest)]
    public class FindOpponentRequestHandler : IPacketHandler
    {
        private readonly Matchmaker _matchmaker;
        private readonly UsersManager _usersManager;

        public FindOpponentRequestHandler(Matchmaker matchmaker, UsersManager gameManager)
        {
            _matchmaker = matchmaker;
            _usersManager = gameManager;
        }

        public void Handle(INetPacket packet, int connectionId)
        {
            var connection = _usersManager.GetConnection(connectionId);
            _matchmaker.RegisterPlayer(connection);
        }
    }
}
