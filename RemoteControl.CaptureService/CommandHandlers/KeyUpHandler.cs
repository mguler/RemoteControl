using RemoteControl.Shared;
using System.Net.Sockets;

namespace RemoteControl.CaptureService.CommandHandlers
{
    public class KeyUpHandler : IHandler
    {
        public byte Command { get; set; } = ControlCommand.KEY_UP;

        public void Handle(UdpClient udp, UdpReceiveResult result)
        {

            var data = result.Buffer;
            var key = data[12];
            Win32.keybd_event(key, 0, Win32.KEYEVENTF_KEYUP, UIntPtr.Zero);
            Console.WriteLine($"key : {(char)data[12]} , event : keyup");
        }
    }
}
