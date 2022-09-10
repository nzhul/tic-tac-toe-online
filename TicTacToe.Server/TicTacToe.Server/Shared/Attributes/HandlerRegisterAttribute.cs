using TicTacToe.Server.Shared;

namespace TicTacToe.Server.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class HandlerRegisterAttribute : Attribute
    {
        public HandlerRegisterAttribute(PacketType type)
        {
            PacketType = type;
        }

        public PacketType PacketType { get; }
    }
}
