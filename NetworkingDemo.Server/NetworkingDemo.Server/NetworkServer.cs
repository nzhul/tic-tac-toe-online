using LiteNetLib;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NetworkingDemo.Server
{
    public class NetworkServer : INetEventListener
    {
        private const int MAX_USERS_COUNT = 100;
        private NetManager _netManager;
        private Dictionary<int, NetPeer> _connections;

        public void Start()
        {
            _connections = new Dictionary<int, NetPeer>();
            _netManager = new NetManager(this)
            {
                DisconnectTimeout = 100000
            };

            _netManager.Start(9050);

            Console.WriteLine("Server listening on port 9050");
        }

        public void PollEvents()
        {
            _netManager.PollEvents();
        }

        public void SendClient(string data)
        {

        }

        #region Interface methods
        public void OnConnectionRequest(ConnectionRequest request)
        {
            Console.WriteLine($"Incomming connection from {request.RemoteEndPoint}");
            request.Accept();

            // todo; check if MAX_USERS_COUNT is reached.
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            //throw new NotImplementedException();
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
            //throw new NotImplementedException();
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
        {
            var data = Encoding.UTF8.GetString(reader.RawData);
            Console.WriteLine($"Data received from client: '{data}'");

            // reply to client
            var reply = "General Kenobi";
            var bytes = Encoding.UTF8.GetBytes(reply);
            peer.Send(bytes, DeliveryMethod.ReliableOrdered);
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            //throw new NotImplementedException();
        }

        public void OnPeerConnected(NetPeer peer)
        {
            Console.WriteLine($"Client connected to server: {peer.EndPoint}. Id: {peer.Id}");
            _connections.Add(peer.Id, peer);
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Console.WriteLine($"{peer.EndPoint} disconnected!");
            _connections.Remove(peer.Id);
        }
        #endregion
    }
}
