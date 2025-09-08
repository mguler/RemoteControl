using RemoteControl.Shared;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Net;

namespace RemoteControl.IntermediateServer
{
    internal class ServerImpl : IServer
    {

        // ID -> last frame bytes
        public ConcurrentDictionary<string, byte[]> LatestFrames { get; private set; } = new ConcurrentDictionary<string, byte[]>();
        // ID -> subscriber endpoints
        public ConcurrentDictionary<string, ConcurrentBag<IPEndPoint>> Subscribers { get; private set; } = new ConcurrentDictionary<string, ConcurrentBag<IPEndPoint>>();
        // ID -> capture server endpoint
        public ConcurrentDictionary<string, IPEndPoint> CaptureServers { get; private set; } = new ConcurrentDictionary<string, IPEndPoint>();
        public ConcurrentDictionary<byte, IHandler> Handlers { get; private set; } = new ConcurrentDictionary<byte, IHandler>();


        public async Task Start(int port)
        {
            using var udp = new UdpClient(port);

            try
            {
                while (true)
                {

                    var result = await udp.ReceiveAsync();
                    _ = Task.Run(() => HandlePacket(udp, result));
                }
            }
            catch (SocketException ex)
            {
                throw;
            }
        }

        private void HandlePacket(UdpClient udp, UdpReceiveResult result)
        {
            var data = result.Buffer;
            var remoteEP = result.RemoteEndPoint;

            if (!Handlers.ContainsKey(data[0]))
            {
                Console.WriteLine($"[UNKNOWN] from {remoteEP}");
                return;
            }

            Handlers[data[0]].Handle(udp, result);
        }

        public void SendFrame(UdpClient udp, string id, byte[] payload, IPEndPoint target)
        {
            //var hdrBytes = Encoding.UTF8.GetBytes($"FRAME {id} {payload.Length}\n");
            //var packet = new byte[hdrBytes.Length + payload.Length];
            //Buffer.BlockCopy(hdrBytes, 0, packet, 0, hdrBytes.Length);
            //Buffer.BlockCopy(payload, 0, packet, hdrBytes.Length, payload.Length);
            //udp.Send(packet, packet.Length, target);
        }

        public void Dispose()
        {

        }
    }
}
