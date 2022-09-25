using Microsoft.Extensions.Logging;
using NetworkShared;
using NetworkShared.Attributes;
using NetworkShared.Models;
using NetworkShared.Packets.ClientServer;
using NetworkShared.Packets.ServerClient;
using System;
using TicTacToe.Server.Games;
using TicTacToe.Server.Utilities;

namespace TicTacToe.Server.PacketHandlers
{
    [HandlerRegister(PacketType.MarkCellRequest)]
    public class MarkCellRequestHandler : IPacketHandler
    {
        private readonly GamesManager _gamesManager;
        private readonly UsersManager _usersManager;
        private readonly NetworkServer _server;
        private readonly ILogger<MarkCellRequestHandler> _logger;

        public MarkCellRequestHandler(
            GamesManager gamesManager,
            UsersManager usersManager,
            NetworkServer server,
            ILogger<MarkCellRequestHandler> logger)
        {
            _gamesManager = gamesManager;
            _usersManager = usersManager;
            _server = server;
            _logger = logger;
        }

        public void Handle(INetPacket packet, int connectionId)
        {
            var msg = (Net_MarkCellRequest)packet;
            var connection = _usersManager.GetConnection(connectionId);
            var userId = connection.User.Id;
            var game = _gamesManager.FindGame(userId);

            // 1. Validate
            Validate(msg.Index, userId, game);

            var outcome = game.MarkCell(msg.Index);

            var rmsg = new Net_OnMarkCell()
            {
                Actor = userId,
                Index = msg.Index,
                Outcome = outcome,
            };

            var opponentId = game.GetOpponent(userId);
            var opponentConnection = _usersManager.GetConnection(opponentId);

            _server.SendClient(connection.ConnectionId, rmsg);
            _server.SendClient(opponentConnection.ConnectionId, rmsg);

            _logger.LogInformation($"`{userId}` marked cell at index `{msg.Index}`!");

            if (outcome == MarkOutcome.None)
            {
                game.SwitchCurrentPlayer();
                return;
            };

            if (outcome == MarkOutcome.Win)
            {
                game.AddWin(userId);
                _usersManager.IncreaseScore(userId);

                _logger.LogInformation($"`{userId}` is a winner! Increasing score and win counter!");
            }

            //// TODO: DELETE THIS CODE AND DO IT ON CLIENT-SIDE
            //Task.Run(() =>
            //{
            //    var isDraw = outcome == MarkOutcome.Draw;
            //    var endRoundMsg = new Net_OnEndRound
            //    {
            //        Winner = isDraw ? "None" : userId,
            //        IsDraw = outcome == MarkOutcome.Draw
            //    };

            //    Thread.Sleep(2000);
            //    _server.SendClient(connection.ConnectionId, endRoundMsg);
            //    _server.SendClient(opponentConnection.ConnectionId, endRoundMsg);

            //    _logger.LogInformation("AFTER!");
            //    _logger.LogInformation($"OnEndRound message has been send after 1 second delay!");
            //});

            //_logger.LogInformation("BEFORE!");
        }

        private void Validate(byte index, string actor, Game game)
        {
            if (game.CurrentUser != actor)
            {
                throw new ArgumentException($"[Bad Request] actor `{actor}` is not the current user!");
            }

            var (row, col) = BasicExtensions.GetRowCol(index);
            if (game.Grid[row, col] != 0)
            {
                throw new ArgumentException($"[Bad Request] cell with index `{index}` at row: `{row}` and col `{col}` is already marked!");
            }
        }
    }
}
