using System;
using TicTacToe.Server.Games;

namespace TicTacToe.Server.Matchmaking
{
    public class MMRequest
    {
        public ServerConnection Connection { get; set; }

        public DateTime SearchStart { get; set; }

        public bool MatchFound { get; set; }
    }
}
