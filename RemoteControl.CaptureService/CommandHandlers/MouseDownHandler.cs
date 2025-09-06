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
            using var ms = new MemoryStream(data);
            using var binaryReader = new BinaryReader(ms);

            ms.Position = 12;

            var viewMode = binaryReader.ReadInt16();
            var h = binaryReader.ReadInt32();
            var w = binaryReader.ReadInt32();
            var x = binaryReader.ReadInt32();
            var y = binaryReader.ReadInt32();
            var button = binaryReader.ReadInt32(); 

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
