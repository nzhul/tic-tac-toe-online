using Microsoft.Extensions.Logging;
using NetworkShared;
using NetworkShared.Attributes;
using NetworkShared.Packets.ServerClient;
using TicTacToe.Server.Data;
using TicTacToe.Server.Game;

namespace TicTacToe.Server.PacketHandlers
{
    [HandlerRegister(PacketType.ServerStatusRequest)]
    public class ServerStatusRequestHandler : IPacketHandler
    {
        private readonly UsersManager _gameManager;
        private readonly NetworkServer _server;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<ServerStatusRequestHandler> _logger;

        public ServerStatusRequestHandler(
            UsersManager gameManager,
            NetworkServer networkServer,
            IUserRepository userRepository,
            ILogger<ServerStatusRequestHandler> logger)
        {
            _gameManager = gameManager;
            _server = networkServer;
            _userRepository = userRepository;
            _logger = logger;
        }

        public void Handle(INetPacket packet, int connectionId)
        {
            var rmsg = new Net_OnServerStatus
            {
                PlayersCount = _userRepository.GetTotalCount(),
                TopPlayers = _gameManager.GetTopPlayers()
            };

            _logger.LogInformation($"Sending back OnServerStatusRequest msg to connection {connectionId}");
            _server.SendClient(connectionId, rmsg);
        }
    }
}