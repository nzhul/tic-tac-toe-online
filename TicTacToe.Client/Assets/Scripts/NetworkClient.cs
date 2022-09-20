using LiteNetLib;
using LiteNetLib.Utils;
using NetworkShared;
using NetworkShared.Registries;
using NetworkShared.Shared;
using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace TTT
{
    public class NetworkClient : MonoBehaviour, INetEventListener
    {

        private NetManager _netManager;
        private NetPeer _server;
        private NetDataWriter _writer;
        private PacketRegistry _packetRegistry;
        private HandlerRegistry _handlerRegistry;

        public event Action OnServerConnected;

        #region Singleton
        private static NetworkClient _instance;

        public static NetworkClient Instance
        {
            get
            {
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }
        #endregion

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            _packetRegistry = new PacketRegistry();
            _handlerRegistry = new HandlerRegistry();
            _writer = new NetDataWriter();
            _netManager = new NetManager(this);
            _netManager.SimulateLatency = true;
            //_netManager.SimulationMinLatency = 200;
            //_netManager.SimulationMaxLatency = 500;
            _netManager.DisconnectTimeout = 100000; // Default is 5000
            _netManager.Start();
        }

        public void SendServer<T>(T packet, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered) where T : INetSerializable
        {
            if (_server == null)
                return;
            _writer.Reset();
            packet.Serialize(_writer);
            _server.Send(_writer, deliveryMethod);
        }

        public void Connect()
        {
            _netManager.Connect("localhost", 9050, "");
        }

        private void Update()
        {
            _netManager.PollEvents();
        }

        private void OnDestroy()
        {
            if (_server != null)
            {
                _netManager.Stop();
            }
        }

        private void OnApplicationQuit()
        {
            Disconnect();
        }

        public void Disconnect()
        {
            _netManager.DisconnectAll();
        }

        #region Interface methods

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
        {
            var packetType = (PacketType)reader.GetByte();
            var packet = ResolvePacket(packetType, reader);
            var handler = ResolveHandler(packetType);
            handler.Handle(packet, peer.Id);
            reader.Recycle();
        }

        public void OnPeerConnected(NetPeer peer)
        {
            Debug.Log("[CLIENT] We connected to server " + peer.EndPoint);
            _server = peer;
            OnServerConnected?.Invoke();
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            //throw new System.NotImplementedException();
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            // TODO: litenetlib
            // what happens when the server is dead ?
            // detect server disconnect and redirect the user to MainMenu ?
            //throw new System.NotImplementedException();

            Debug.Log("Lost connection to server!");
        }

        public void OnConnectionRequest(ConnectionRequest request)
        {
            // not-implemented
            // we do not expect anyone to connect to us.
            // we as client are the one that is connecting
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            //throw new System.NotImplementedException();
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
            //throw new System.NotImplementedException();
        }

        #endregion

        private IPacketHandler ResolveHandler(PacketType packetType)
        {
            var handlerType = _handlerRegistry.Handlers[packetType];
            return (IPacketHandler)Activator.CreateInstance(handlerType);
        }

        private INetPacket ResolvePacket(PacketType packetType, NetPacketReader reader)
        {
            var type = _packetRegistry.PacketTypes[packetType];
            var packet = (INetPacket)Activator.CreateInstance(type);
            packet.Deserialize(reader);
            return packet;
        }
    }
}
