using LiteNetLib;
using NetworkShared.Packets.ServerClient;
using System.Collections.Generic;
using System.Linq;
using TicTacToe.Server.Data;
using TicTacToe.Server.Matchmaking;

namespace TicTacToe.Server.Games
{
    public class UsersManager
    {
        private Dictionary<int, ServerConnection> _connections;
        private readonly IUserRepository _userRepository;

        private readonly Matchmaker _matchmaker;
        private readonly GamesManager _gamesManager;
        private readonly NetworkServer _server;

        public UsersManager(
            IUserRepository userRepository,
            Matchmaker matchmaker,
            GamesManager gamesManager,
            NetworkServer server
            )
        {
            _connections = new Dictionary<int, ServerConnection>();
            _userRepository = userRepository;
            _matchmaker = matchmaker;
            _gamesManager = gamesManager;
            _server = server;
        }

        public PlayerNetDto[] GetTopPlayers()
        {
            return _userRepository.GetQuery()
                .OrderByDescending(u => u.Score)
                .Select(u => new PlayerNetDto
                {
                    Username = u.Id,
                    Score = u.Score,
                    IsOnline = u.IsOnline,
                })
                .Take(9)
                .ToArray();
        }

        public ushort GetConnectionsCount()
        {
            return (ushort)_connections.Count;
        }

        public void AddConnection(NetPeer peer)
        {
            _connections.Add(peer.Id, new ServerConnection
            {
                ConnectionId = peer.Id,
                Peer = peer,
            });
        }

        public bool LoginOrRegister(int connectionId, string username, string password)
        {
            var dbUser = _userRepository.Get(username);

            if (dbUser != null)
            {
                if (dbUser.Password != password)
                {
                    return false;
                }
            }

            if (dbUser == null)
            {
                var newUser = new User
                {
                    Id = username,
                    Password = password,
                    IsOnline = true,
                    Score = 0
                };

                _userRepository.Add(newUser);
                dbUser = newUser;
            }

            if (_connections.ContainsKey(connectionId))
            {
                dbUser.IsOnline = true;
                _connections[connectionId].User = dbUser;
            }

            return true;
        }

        public ServerConnection GetConnection(int peerId)
        {
            return _connections[peerId];
        }

        public ServerConnection GetConnection(string userId)
        {
            return _connections.FirstOrDefault(x => x.Value.User.Id == userId).Value;
        }

        public int[] GetOtherConnectionIds(int excludedConnectionId)
        {
            return _connections.Keys.Where(v => v != excludedConnectionId).ToArray();
        }

        public void Disconnect(int peerId)
        {
            var connection = GetConnection(peerId);
            if (connection.User != null)
            {
                var userId = connection.User.Id;
                _userRepository.SetOffline(userId);
                _matchmaker.TryUnregisterPlayer(userId);

                if (_gamesManager.GameExists(userId))
                {
                    var closedGame = _gamesManager.CloseGame(userId);

                    var rmsg = new Net_OnQuitGame()
                    {
                        Quitter = userId
                    };

                    var opponentId = closedGame.GetOpponent(userId);
                    var opponentConn = GetConnection(opponentId);
                    IncreaseScore(opponentId);
                    _server.SendClient(opponentConn.ConnectionId, rmsg);
                }

                NotifyOtherPlayers(peerId);
            }

            _connections.Remove(peerId);
        }

        public void IncreaseScore(string userId)
        {
            var user = _userRepository.Get(userId);
            user.Score += 10;
            _userRepository.Update(user);
        }

        private void NotifyOtherPlayers(int excludedConnectionId)
        {
            var rmsg = new Net_OnServerStatus
            {
                PlayersCount = _userRepository.GetTotalCount(),
                TopPlayers = GetTopPlayers()
            };

            var otherIds = GetOtherConnectionIds(excludedConnectionId);

            foreach (var connectionId in otherIds)
            {
                _server.SendClient(connectionId, rmsg);
            }
        }
    }
}
