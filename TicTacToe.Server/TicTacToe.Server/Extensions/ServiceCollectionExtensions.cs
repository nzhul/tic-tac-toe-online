using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TicTacToe.Server.Attributes;
using TicTacToe.Server.Shared;

namespace TicTacToe.Server.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPacketHandlers(this IServiceCollection services)
        {
            var handlers = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.DefinedTypes)
                .Where(x => !x.IsAbstract && !x.IsInterface && !x.IsGenericTypeDefinition)
                .Where(x => typeof(IPacketHandler).IsAssignableFrom(x))
                .Select(t => (type: t, attr: t.GetCustomAttribute<HandlerRegisterAttribute>()))
                .Where(x => x.attr != null);

            foreach (var (type, attr) in handlers)
            {
                services.AddScoped(type);
            }

            return services;
        }
    }
}
