using RemoteControl.Shared;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Net;

namespace RemoteControl.IntermediateServer
{
    public interface IServer
    {
        ConcurrentDictionary<string, byte[]> LatestFrames { get; }
        ConcurrentDictionary<string, ConcurrentBag<IPEndPoint>> Subscribers { get; }
        ConcurrentDictionary<string, IPEndPoint> CaptureServers { get; }
        ConcurrentDictionary<byte, IHandler> Handlers { get; }
        void SendFrame(UdpClient udp, string id, byte[] payload, IPEndPoint target);
    }
}
