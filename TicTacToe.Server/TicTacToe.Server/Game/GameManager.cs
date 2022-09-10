using LiteNetLib;

namespace TicTacToe.Server.Game
{
    public class GameManager
    {
        private Dictionary<int, ServerConnection> _connections;

        public GameManager()
        {
            _connections = new Dictionary<int, ServerConnection>();
        }

        public int GetConnectionsCount()
        {
            return _connections.Count;
        }

        public void AddConnection(NetPeer peer)
        {
            _connections.Add(peer.Id, new ServerConnection
            {
                ConnectionId = peer.Id,
                Peer = peer,
            });
        }

        public void RegisterPlayer(int connectionId, string username, string password)
        {
            if (_connections.ContainsKey(connectionId))
            {
                _connections[connectionId].Username = username;
                _connections[connectionId].Password = password;
            }
        }

        public ServerConnection GetConnection(int peerId)
        {
            return _connections[peerId];
        }

        public void RemoveConnection(int peerId)
        {
            _connections.Remove(peerId);
        }
    }
}
