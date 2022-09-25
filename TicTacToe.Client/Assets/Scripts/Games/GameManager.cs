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

        public MarkType MyType { get; set; }

        public MarkType OpponentType { get; set; }

        public bool InputsEnabled { get; set; }

        public bool IsMyTurn
        {
            get
            {
                if (_activeGame == null)
                {
                    return false;
                }

                if (_activeGame.CurrentUser != MyUsername)
                {
                    return false;
                }

                return true;
            }
        }

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
                Id = gameId,
                XUser = xUser,
                OUser = oUser,
                StartTime = DateTime.Now,
                CurrentUser = xUser,
            };

            if (MyUsername == xUser)
            {
                MyType = MarkType.X;
                OpponentUsername = oUser;
                OpponentType = MarkType.O;
            }
            else
            {
                MyType = MarkType.O;
                OpponentUsername = xUser;
                OpponentType = MarkType.X;
            }

            InputsEnabled = true;
        }

        public class Game
        {
            public Guid? Id { get; set; }

            public string XUser { get; set; }

            public string OUser { get; set; }

            public string CurrentUser { get; set; }

            public DateTime StartTime { get; set; }

            public DateTime EndTime { get; set; }

            public MarkType GetPlayerType(string userId)
            {
                if (userId == XUser)
                {
                    return MarkType.X;
                }
                else
                {
                    return MarkType.O;
                }
            }

            public void SwitchCurrentPlayer()
            {
                CurrentUser = GetOpponent(CurrentUser);
            }

            public void Reset()
            {
                CurrentUser = XUser;
            }

            private string GetOpponent(string otherUserId)
            {
                if (otherUserId == XUser)
                {
                    return OUser;
                }
                else
                {
                    return XUser;
                }
            }
        }
    }
}
