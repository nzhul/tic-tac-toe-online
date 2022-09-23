using NetworkShared.Models;
using System;
using UnityEngine;

namespace Assets.Scripts.Games
{
    public class GameManager : MonoBehaviour
    {
        #region Singleton
        private static GameManager _instance;

        public static GameManager Instance
        {
            get
            {
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }
        #endregion

        public string MyUsername { get; set; }

        public string OpponentUsername { get; set; }

        public PlayerType MyType { get; set; }

        public PlayerType OpponentType { get; set; }

        private Game _activeGame;

        public Game ActiveGame
        {
            get
            {
                return _activeGame;
            }
        }

        public void RegisterGame(Guid gameId, string xUser, string oUser)
        {
            _activeGame = new Game()
            {
                GameId = gameId,
                XUser = xUser,
                OUser = oUser,
                StartTime = DateTime.Now,
            };

            if (MyUsername == xUser)
            {
                MyType = PlayerType.X;
                OpponentUsername = oUser;
                OpponentType = PlayerType.O;
            }
            else
            {
                MyType = PlayerType.O;
                OpponentUsername = xUser;
                OpponentType = PlayerType.X;
            }
        }

        public class Game
        {
            public Guid? GameId { get; set; }

            public string XUser { get; set; }

            public string OUser { get; set; }

            public DateTime StartTime { get; set; }

            public DateTime EndTime { get; set; }

            public PlayerType GetPlayerType(string userId)
            {
                if (userId == XUser)
                {
                    return PlayerType.X;
                }
                else
                {
                    return PlayerType.O;
                }
            }
        }
    }
}
