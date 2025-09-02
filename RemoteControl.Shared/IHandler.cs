using System.Net.Sockets;

namespace RemoteControl.Shared
{
    public interface IHandler
    {
            byte Command { get; set; }
            void Handle(UdpClient udp, UdpReceiveResult result);
    }
}
