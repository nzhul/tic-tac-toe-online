using Microsoft.Extensions.DependencyInjection;
using TicTacToe.Server;
using TicTacToe.Server.Infrastructure;

var serviceProvider = Container.Confugure();
var _server = serviceProvider.GetRequiredService<NetworkServer>();
_server.Start();

while (true)
{
    _server.PollEvents();
    Thread.Sleep(15);
}