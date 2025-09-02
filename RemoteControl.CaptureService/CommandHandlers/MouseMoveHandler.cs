using RemoteControl.Shared;
using System.Net.Sockets;

namespace RemoteControl.CaptureService.CommandHandlers
{
    public class MouseMoveHandler : IHandler
    {
        public byte Command { get; set; } = ControlCommand.MOUSE_MOVE;

        public void Handle(UdpClient udp, UdpReceiveResult result)
        {
            var data = result.Buffer;

            var viewMode = data[12];
            var h = BitConverter.ToInt32(data, 14);
            var w = BitConverter.ToInt32(data, 18);
            var x = BitConverter.ToInt32(data, 22);
            var y = BitConverter.ToInt32(data, 26);

            var bounds = Screen.PrimaryScreen.Bounds;
            var posX = (double)x / w * bounds.Width;
            var posY = (double)y / h * bounds.Height;

            Cursor.Position = new Point((int)posX, (int)posY);

            Console.WriteLine($"x : {posX} , y : {posY} ,  event : move");
        }
    }
}
