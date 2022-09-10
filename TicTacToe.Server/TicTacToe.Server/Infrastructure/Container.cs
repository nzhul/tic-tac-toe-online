using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TicTacToe.Server.Extensions;
using TicTacToe.Server.Game;
using TicTacToe.Server.Registries;

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
            services.AddSingleton<GameManager>();
            services.AddPacketHandlers();
        }
    }
}
