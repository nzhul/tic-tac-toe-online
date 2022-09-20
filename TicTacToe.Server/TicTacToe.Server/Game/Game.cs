using System;

namespace TicTacToe.Server.Game
{
    public class Game
    {
        private PlayerType _currentPlayer;

        public Game(string xUser, string oUser)
        {
            Id = Guid.NewGuid();
            StartTime = DateTime.UtcNow;
            XPlayer = xUser;
            OPlayer = oUser;
        }

        public Guid Id { get; }

        public DateTime StartTime { get; }

        public string OPlayer { get; }

        public string XPlayer { get; }

        public ushort OWins { get; set; }

        public ushort XWins { get; set; }

        public PlayerType GetCurrentPlayer()
        {
            return _currentPlayer;
        }

        void SwitchCurrentPlayer()
        {
            if (_currentPlayer == PlayerType.O)
            {
                _currentPlayer = PlayerType.X;
            }
            else
            {
                _currentPlayer = PlayerType.O;
            }
        }
    }

    public enum PlayerType
    {
        X,
        O
    }
}
