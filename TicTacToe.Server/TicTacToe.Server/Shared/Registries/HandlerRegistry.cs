using System.Reflection;
using TicTacToe.Server.Attributes;
using TicTacToe.Server.Shared;

namespace TicTacToe.Server.Registries
{
    public class HandlerRegistry
    {
        private Dictionary<PacketType, Type> _handlers = new Dictionary<PacketType, Type>();

        public Dictionary<PacketType, Type> Handlers
        {
            get
            {
                if (_handlers.Count == 0)
                {
                    Initialize();
                }

                return _handlers;
            }
        }

        public void Initialize()
        {
            if (_handlers.Count > 0)
            {
                return;
            }

            var handlers = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.DefinedTypes)
                .Where(x => !x.IsAbstract && !x.IsInterface && !x.IsGenericTypeDefinition)
                .Where(x => typeof(IPacketHandler).IsAssignableFrom(x))
                .Select(t => (type: t, attr: t.GetCustomAttribute<HandlerRegisterAttribute>()))
                .Where(x => x.attr != null);

            foreach (var (type, attr) in handlers)
            {
                if (!_handlers.ContainsKey(attr.PacketType))
                {
                    _handlers[attr.PacketType] = type;
                }
                else
                {
                    throw new Exception($"Multiple handlers for `{attr.PacketType}` packet type detected! Only one handler per packet type is supported!");
                }
            }
        }
    }
}
