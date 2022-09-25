using NetworkShared;
using NetworkShared.Attributes;
using NetworkShared.Packets.ServerClient;
using TicTacToe.Server.Data;
using TicTacToe.Server.Games;

namespace TicTacToe.Server.PacketHandlers
{
    [HandlerRegister(PacketType.ServerStatusRequest)]
    public class ServerStatusRequestHandler : IPacketHandler
    {
        private readonly UsersManager _gameManager;
        private readonly NetworkServer _server;
        private readonly IUserRepository _userRepository;

        public ServerStatusRequestHandler(
            UsersManager gameManager,
            NetworkServer networkServer,
            IUserRepository userRepository)
        {
            _gameManager = gameManager;
            _server = networkServer;
            _userRepository = userRepository;
        }

        public void Handle(INetPacket packet, int connectionId)
        {
            var rmsg = new Net_OnServerStatus
            {
                PlayersCount = _userRepository.GetTotalCount(),
                TopPlayers = _gameManager.GetTopPlayers()
            };

            _server.SendClient(connectionId, rmsg);
        }
    }
}