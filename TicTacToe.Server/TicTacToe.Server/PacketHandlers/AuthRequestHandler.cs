using Microsoft.Extensions.Logging;
using NetworkShared;
using NetworkShared.Attributes;
using NetworkShared.Packets.ClientServer;
using NetworkShared.Packets.ServerClient;
using TicTacToe.Server.Data;
using TicTacToe.Server.Games;

namespace TicTacToe.Server.PacketHandlers
{
    [HandlerRegister(PacketType.AuthRequest)]
    public class AuthRequestHandler : IPacketHandler
    {
        private readonly ILogger<AuthRequestHandler> _logger;
        private readonly UsersManager _usersManager;
        private readonly NetworkServer _server;
        private readonly IUserRepository _userRepository;

        public AuthRequestHandler(
            ILogger<AuthRequestHandler> logger,
            UsersManager gameManager,
            NetworkServer server,
            IUserRepository userRepository)
        {
            _logger = logger;
            _usersManager = gameManager;
            _server = server;
            _userRepository = userRepository;
        }

        public void Handle(INetPacket packet, int connectionId)
        {
            var msg = (Net_AuthRequest)packet;

            _logger.LogInformation($"Received login request for user: {msg.Username} with pass: {msg.Password}");

            var loginSuccess = _usersManager.LoginOrRegister(connectionId, msg.Username, msg.Password);

            INetPacket rmsg;

            // Additional fake slow.
            //Thread.Sleep(TimeSpan.FromMilliseconds(500));

            if (loginSuccess)
            {
                rmsg = new Net_OnAuth();
            }
            else
            {
                rmsg = new Net_OnAuthFail();
            }

            _server.SendClient(connectionId, rmsg);

            if (loginSuccess)
            {
                NotifyOtherPlayers(connectionId);
            }
        }

        private void NotifyOtherPlayers(int excludedConnectionId)
        {
            var rmsg = new Net_OnServerStatus
            {
                PlayersCount = _userRepository.GetTotalCount(),
                TopPlayers = _usersManager.GetTopPlayers()
            };

            var otherIds = _usersManager.GetOtherConnectionIds(excludedConnectionId);

            foreach (var connectionId in otherIds)
            {
                _server.SendClient(connectionId, rmsg);
            }
        }
    }
}
