using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using TicTacToe.Server;
using TicTacToe.Server.Games;
using TicTacToe.Server.Infrastructure;

var serviceProvider = Container.Confugure();
var _server = serviceProvider.GetRequiredService<NetworkServer>();
_server.Start();

while (true)
{
#if DEBUG
    if (Console.KeyAvailable)
    {
        var key = Console.ReadKey(true);
        HandleConsoleCommand(key.Key);
    }
#endif
    _server.PollEvents();
    Thread.Sleep(15);
}

void HandleConsoleCommand(ConsoleKey key)
{
    var usersManager = serviceProvider.GetRequiredService<UsersManager>();
    var gamesManager = serviceProvider.GetRequiredService<GamesManager>();

    switch (key)
    {
        case ConsoleKey.S:
            Console.WriteLine($"Connections: {usersManager.GetConnectionsCount()}");
            break;
        case ConsoleKey.G:
            Console.WriteLine($"Games: {gamesManager.GetGamesCount()}");
            break;
    }
}