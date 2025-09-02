using RemoteControl.Shared;
using System.Net.Sockets;
using System.Text;

namespace RemoteControl.IntermediateServer.CommandHandlers
{
    public class Subscribe : IHandler
    {
        public byte Command { get; set; } = Shared.Command.SUBSCRIBE;

        private IServer _server;
        public Subscribe(IServer server)
        {
            _server = server;
        }

        public void Handle(UdpClient udp, UdpReceiveResult result)
        {
            var data = result.Buffer;
            var remoteEP = result.RemoteEndPoint;
            var id = Encoding.UTF8.GetString(data, 1, 10);

            if (_server.Subscribers.ContainsKey(id))
            {
                _server.Subscribers[id].Add(remoteEP);
                Console.WriteLine($"[SUBSCRIBE] {remoteEP} subscribed to {id}");
                // Send latest frame if available
                if (_server.LatestFrames.TryGetValue(id, out var last) && last != null)
                _server.SendFrame(udp, id, last, remoteEP);
            }
        }
    }
}