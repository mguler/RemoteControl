using RemoteControl.Shared;
using System.Net.Sockets;
using System.Text;

namespace RemoteControl.IntermediateServer.CommandHandlers
{
    public class Frame:IHandler
    {
        public byte Command { get; set; } = Shared.Command.FRAME;
        private IServer _server;

        public Frame(IServer server)
        {
            _server = server;
        }
        public void Handle(UdpClient udp, UdpReceiveResult result)
        {
            var data = result.Buffer;
            var id = Encoding.UTF8.GetString(data, 1, 10);
            if (_server.Subscribers.TryGetValue(id, out var bag))
            {
                foreach (var subscriberEndPoint in bag)
                {
                    udp.Send(data, data.Length, subscriberEndPoint);
                }
            }

        }
    }
}
