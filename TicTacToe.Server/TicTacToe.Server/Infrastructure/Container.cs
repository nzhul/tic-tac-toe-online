using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetworkShared.Registries;
using System;
using TicTacToe.Server.Data;
using TicTacToe.Server.Extensions;
using TicTacToe.Server.Game;
using TicTacToe.Server.Matchmaking;

namespace TicTacToe.Server.Infrastructure
{
    public static class Container
    {
        public static IServiceProvider Confugure()
        {
            var services = new ServiceCollection();

            ConfigureServices(services);

            return services.BuildServiceProvider();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(c => c.AddSimpleConsole());
            services.AddSingleton<NetworkServer>();
            services.AddSingleton<PacketRegistry>();
            services.AddSingleton<HandlerRegistry>();
            services.AddSingleton<UsersManager>();
            services.AddSingleton<GamesManager>();
            services.AddSingleton<Matchmaker>();
            services.AddSingleton<IUserRepository, InMemoryUserRepository>(); // This should be AddScoped with using real database.
            services.AddPacketHandlers();
        }
    }
}
