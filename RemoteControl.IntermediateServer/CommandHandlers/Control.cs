using RemoteControl.Shared;
using System.Net.Sockets;
using System.Text;

namespace RemoteControl.IntermediateServer.CommandHandlers
{
    public class Control : IHandler
    {
        public byte Command { get; set; } = Shared.Command.CONTROL;

        private IServer _server;

        public Control(IServer server)
        {
            _server = server;
            
        }
        public void Handle(UdpClient udp, UdpReceiveResult result)
        {
            var data = result.Buffer;
            var id = Encoding.UTF8.GetString(data, 2, 10);

            if (_server.CaptureServers.TryGetValue(id, out var csEP))
            {
                // Forward entire CONTROL packet to capture server
                udp.Send(data, data.Length, csEP);
            }
        }
    }
}