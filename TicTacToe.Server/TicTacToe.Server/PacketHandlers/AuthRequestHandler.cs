using Microsoft.Extensions.Logging;
using NetworkShared;
using NetworkShared.Attributes;
using NetworkShared.Packets.ClientServer;
using NetworkShared.Packets.ServerClient;
using TicTacToe.Server.Game;

namespace TicTacToe.Server.PacketHandlers
{
    [HandlerRegister(PacketType.AuthRequest)]
    public class AuthRequestHandler : IPacketHandler
    {
        private readonly ILogger<AuthRequestHandler> _logger;
        private readonly UsersManager _gameManager;
        private readonly NetworkServer _server;

        public AuthRequestHandler(
            ILogger<AuthRequestHandler> logger,
            UsersManager gameManager,
            NetworkServer server)
        {
            _logger = logger;
            _gameManager = gameManager;
            _server = server;
        }

        public void Handle(INetPacket packet, int connectionId)
        {
            var msg = (Net_AuthRequest)packet;

            _logger.LogInformation($"Received login request for user: {msg.Username} with pass: {msg.Password}");

            var loginSuccess = _gameManager.LoginOrRegister(connectionId, msg.Username, msg.Password);

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
        }
    }
}
