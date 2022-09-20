using System;
using System.Collections.Generic;

namespace TicTacToe.Server.Game
{
    public class GamesManager
    {
        private List<Game> _games;

        public GamesManager()
        {
            _games = new List<Game>();
        }

        public Guid RegisterGame(string xUser, string oUser)
        {
            var newGame = new Game(xUser, oUser);

            _games.Add(newGame);

            return newGame.Id;
        }
    }
}
