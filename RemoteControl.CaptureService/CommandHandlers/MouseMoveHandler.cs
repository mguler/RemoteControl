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
            using var ms = new MemoryStream(data);
            using var binaryReader = new BinaryReader(ms);

            ms.Position = 12;

            var viewMode = binaryReader.ReadInt16();
            var h = binaryReader.ReadInt32();
            var w = binaryReader.ReadInt32();
            var x = binaryReader.ReadInt32();
            var y = binaryReader.ReadInt32();

            var bounds = Screen.PrimaryScreen.Bounds;
            var posX = (double)x / w * bounds.Width;
            var posY = (double)y / h * bounds.Height;

            Cursor.Position = new Point((int)posX, (int)posY);

            Console.WriteLine($"x : {posX} , y : {posY} ,  event : move");
        }
    }
}
