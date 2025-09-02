using RemoteControl.Shared;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace RemoteControl.IntermediateServer.CommandHandlers
{
    public class Register : IHandler
    {
        public byte Command { get; set; } = Shared.Command.REGISTER;

        private readonly string _idChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private readonly Random _rnd = new Random();

        private IServer _server;

        public Register(IServer server)
        {
            _server = server;
        }
        public void Handle(UdpClient udp, UdpReceiveResult result) {

            // Generate unique 10-char ID
            string id;
            var remoteEP = result.RemoteEndPoint;

            do
            {
                id = new string(Enumerable.Range(0, 10)
                    .Select(_ => _idChars[_rnd.Next(_idChars.Length)])
                    .ToArray());
            } while (!_server.LatestFrames.TryAdd(id, null));

            // Record capture server endpoint
            _server.CaptureServers[id] = remoteEP;
            // Initialize subscribers list
            _server.Subscribers[id] = new ConcurrentBag<IPEndPoint>();

            // Reply with ID
            var reply = Encoding.UTF8.GetBytes($" {id}");
            reply[0] = Shared.Command.ID;
            udp.Send(reply, reply.Length, remoteEP);
            Console.WriteLine($"[REGISTER] {remoteEP} -> ID {id}");
        }
    }
}
