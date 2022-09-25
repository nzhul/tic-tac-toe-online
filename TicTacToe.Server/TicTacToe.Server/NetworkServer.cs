using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetworkShared;
using NetworkShared.Registries;
using System;
using System.Net;
using System.Net.Sockets;
using TicTacToe.Server.Games;

namespace TicTacToe.Server
{
    public class NetworkServer : INetEventListener
    {
        private const int MAX_USERS_COUNT = 100;

        private readonly ILogger<NetworkServer> _logger;
        private readonly IServiceProvider _serviceProvider;
        private UsersManager _usersManager;
        private NetManager _netManager;
        private readonly NetDataWriter _cachedWriter = new NetDataWriter();

        public NetworkServer(
            ILogger<NetworkServer> logger,
            IServiceProvider provider)
        {
            _logger = logger;
            _serviceProvider = provider;
        }

        #region Interface methods

        public void OnConnectionRequest(ConnectionRequest request)
        {
            _logger.LogInformation($"Incomming connection from {request.RemoteEndPoint}");

            if (_usersManager.GetConnectionsCount() < MAX_USERS_COUNT)
            {
                request.Accept();
                return;
            }

            Console.WriteLine("Connection rejected! Server is FULL!");
            request.Reject();
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                try
                {
                    var packetType = (PacketType)reader.GetByte();
                    var packet = ResolvePacket(packetType, reader);
                    var handler = ResolveHandler(packetType);

                    handler.Handle(packet, peer.Id);

                    reader.Recycle();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message of type XX");
                }
            }
        }

        public void OnPeerConnected(NetPeer peer)
        {
            _logger.LogInformation($"Client connected to server: {peer.EndPoint}. Id: {peer.Id}");
            _usersManager.AddConnection(peer);
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            var connection = _usersManager.GetConnection(peer.Id);
            _netManager.DisconnectPeer(peer);
            _usersManager.Disconnect(peer.Id);
            _logger.LogInformation($"{connection?.User?.Id} disconnected: {peer.EndPoint}");
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            throw new NotImplementedException();
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            //throw new NotImplementedException();
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
            //throw new NotImplementedException();
        }

        #endregion

        public void Start()
        {
            _netManager = new NetManager(this)
            {
                SimulateLatency = true,
                //SimulationMinLatency = 200,
                //SimulationMaxLatency = 500,
                DisconnectTimeout = 100000 // TODO: use config for this. Default is 5000
            };

            _netManager.Start(9050);

            // dido: we are intentionally resolving the usersManager here to avoid circular dependency problem.
            _usersManager = _serviceProvider.GetRequiredService<UsersManager>();

            _logger.LogInformation("Server listening on port 9050");
        }

        public void PollEvents()
        {
            _netManager.PollEvents();
        }


        public void SendClient(int peerId, INetPacket packet, DeliveryMethod method = DeliveryMethod.ReliableOrdered)
        {
            var peer = _usersManager.GetConnection(peerId).Peer;
            peer.Send(WriteSerializable(packet), method);
        }


        private IPacketHandler ResolveHandler(PacketType packetType)
        {
            var registry = _serviceProvider.GetRequiredService<HandlerRegistry>();
            var type = registry.Handlers[packetType];
            return (IPacketHandler)_serviceProvider.GetRequiredService(type);
        }

        private INetPacket ResolvePacket(PacketType packetType, NetPacketReader reader)
        {
            var registry = _serviceProvider.GetRequiredService<PacketRegistry>();
            var type = registry.PacketTypes[packetType];
            var packet = (INetPacket)Activator.CreateInstance(type);
            packet.Deserialize(reader);
            return packet;
        }

        private NetDataWriter WriteSerializable(INetPacket packet)
        {
            _cachedWriter.Reset();
            packet.Serialize(_cachedWriter);
            return _cachedWriter;
        }
    }
}
