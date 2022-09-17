using LiteNetLib;

namespace TicTacToe.Server.Game
{
    public class ServerConnection
    {
        public int ConnectionId { get; set; }

        public string Username { get; set; }

        public int Score { get; set; }

        public string Password { get; set; }

        public NetPeer Peer { get; set; }
    }
}
