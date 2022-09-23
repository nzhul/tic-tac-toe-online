using LiteNetLib;
using NetworkShared.Packets.ServerClient;
using System.Collections.Generic;
using System.Linq;
using TicTacToe.Server.Data;
using TicTacToe.Server.Matchmaking;

namespace TicTacToe.Server.Game
{
    public class UsersManager
    {
        private Dictionary<int, ServerConnection> _connections;
        private readonly IUserRepository _userRepository;

        private readonly Matchmaker _matchmaker;

        public UsersManager(
            IUserRepository userRepository,
            Matchmaker matchmaker
            )
        {
            _connections = new Dictionary<int, ServerConnection>();
            _userRepository = userRepository;
            _matchmaker = matchmaker;
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

        public void Disconnect(int peerId)
        {
            var connection = GetConnection(peerId);
            if (connection.User != null)
            {
                _userRepository.SetOffline(connection.User.Id);
                _matchmaker.TryUnregisterPlayer(connection.User.Id);
            }

            _connections.Remove(peerId);
        }

        public void IncreaseScore(string userId)
        {
            var user = _userRepository.Get(userId);
            user.Score += 10;
            _userRepository.Update(user);
        }
    }
}
