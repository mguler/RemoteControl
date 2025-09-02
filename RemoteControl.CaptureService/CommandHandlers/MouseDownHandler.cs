using RemoteControl.Shared;
using System.Net.Sockets;

namespace RemoteControl.CaptureService.CommandHandlers
{
    public class MouseDownHandler:IHandler
    {
        public byte Command { get; set; } = ControlCommand.MOUSE_DOWN;

        public void Handle(UdpClient udp, UdpReceiveResult result)
        {
            var data = result.Buffer;

            var viewMode = data[12];
            var h = BitConverter.ToInt32(data, 14);
            var w = BitConverter.ToInt32(data, 18);
            var x = BitConverter.ToInt32(data, 22);
            var y = BitConverter.ToInt32(data, 26);
            var button = data[30];

            var bounds = Screen.PrimaryScreen.Bounds;
            var posX = (double)x / w * bounds.Width;
            var posY = (double)y / h * bounds.Height;

            Cursor.Position = new Point((int)posX, (int) posY);

            if (button == MouseButton.LEFT) Win32.mouse_event(Win32.MOUSEEVENTF_LEFTDOWN, 0, 0, 0, UIntPtr.Zero);
            if (button == MouseButton.RIGHT) Win32.mouse_event(Win32.MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, UIntPtr.Zero);


            Console.WriteLine($"x : {posX} , y : {posY} , button : {(button)} , event : down");
        }
    }
}
