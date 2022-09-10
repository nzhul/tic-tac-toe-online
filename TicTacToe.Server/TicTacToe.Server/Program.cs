using Microsoft.Extensions.DependencyInjection;
using TicTacToe.Server;
using TicTacToe.Server.Game;
using TicTacToe.Server.Infrastructure;

var serviceProvider = Container.Confugure();
var _server = serviceProvider.GetRequiredService<NetworkServer>();
_server.Start();

while (true)
{
    //if (Console.KeyAvailable)
    //{
    //    var key = Console.ReadKey(true);
    //    HandleConsoleCommand(key.Key);
    //}

    _server.PollEvents();
    Thread.Sleep(15);
}

void HandleConsoleCommand(ConsoleKey key)
{
    switch (key)
    {
        case ConsoleKey.S:
            var gameManager = serviceProvider.GetRequiredService<GameManager>();
            Console.WriteLine($"Connections: {gameManager.GetConnectionsCount()}");
            break;
    }
}