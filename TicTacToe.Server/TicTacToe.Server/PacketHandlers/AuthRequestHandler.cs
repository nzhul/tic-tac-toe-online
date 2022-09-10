using Microsoft.Extensions.Logging;
using TicTacToe.Server.Attributes;
using TicTacToe.Server.Game;
using TicTacToe.Server.Packets.ClientServer;
using TicTacToe.Server.Packets.ServerClient;
using TicTacToe.Server.Shared;

namespace TicTacToe.Server.PacketHandlers
{
    [HandlerRegister(PacketType.AuthRequest)]
    public class AuthRequestHandler : IPacketHandler
    {
        private readonly ILogger<AuthRequestHandler> _logger;
        private readonly GameManager _gameManager;
        private readonly NetworkServer _server;

        public AuthRequestHandler(
            ILogger<AuthRequestHandler> logger,
            GameManager gameManager,
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

            _gameManager.RegisterPlayer(connectionId, msg.Username, msg.Password);

            var rmsg = new Net_OnAuthRequest();

            _server.SendClient(connectionId, rmsg);
        }
    }
}
