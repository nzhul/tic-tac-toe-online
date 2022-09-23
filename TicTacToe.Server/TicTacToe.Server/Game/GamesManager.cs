using System;
using System.Collections.Generic;
using System.Linq;

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

        public Game FindGame(string username)
        {
            return _games.FirstOrDefault(g => g.XPlayer == username || g.OPlayer == username);
        }

        public Game CloseGame(string username)
        {
            var game = FindGame(username);
            _games.Remove(game);

            return game;
        }
    }
}
