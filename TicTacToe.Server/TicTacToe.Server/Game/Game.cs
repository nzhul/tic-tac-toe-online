using NetworkShared.Models;
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
            CurrentRoundStartTime = DateTime.UtcNow;
            XPlayer = xUser;
            OPlayer = oUser;
            Round = 1;
        }

        public Guid Id { get; }

        public ushort Round { get; set; }

        public DateTime StartTime { get; }

        public DateTime CurrentRoundStartTime { get; set; }

        public string OPlayer { get; }

        public ushort OWins { get; set; }

        public bool OWantRematch { get; set; }

        public string XPlayer { get; }

        public ushort XWins { get; set; }

        public bool XWantRematch { get; set; }

        public PlayerType GetCurrentPlayer()
        {
            return _currentPlayer;
        }

        public string GetOpponent(string otherUserId)
        {
            if (otherUserId == XPlayer)
            {
                return OPlayer;
            }
            else
            {
                return XPlayer;
            }
        }

        public string AddWin(string looserId)
        {
            var opponentId = GetOpponent(looserId);
            var winnerType = GetPlayerType(opponentId);

            if (winnerType == PlayerType.X)
            {
                XWins++;
            }
            else
            {
                OWins++;
            }

            return opponentId;
        }

        public void SetRematchReadiness(string userId)
        {
            var playerType = GetPlayerType(userId);
            if (playerType == PlayerType.X)
            {
                XWantRematch = true;
            }
            else
            {
                OWantRematch = true;
            }
        }

        private PlayerType GetPlayerType(string userId)
        {
            if (userId == XPlayer)
            {
                return PlayerType.X;
            }
            else
            {
                return PlayerType.O;
            }
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

        public void NewRound()
        {
            _currentPlayer = PlayerType.X;
            CurrentRoundStartTime = DateTime.UtcNow;
        }

        public bool BothPlayersReady()
        {
            return XWantRematch && OWantRematch;
        }
    }
}
