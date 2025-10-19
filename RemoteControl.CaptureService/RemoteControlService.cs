using Microsoft.Extensions.Hosting;
using RemoteControl.Shared;
using System.Net.Sockets;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using RemoteControl.Shared.Extensions;
using System.Collections.Concurrent;
namespace RemoteControl.CaptureService
{
    public class RemoteControlService : BackgroundService
    {
        private readonly Dictionary<string, string> _args;
        private readonly ConcurrentDictionary<byte, IHandler> _handlers;
        public RemoteControlService(Dictionary<string,string> args, ConcurrentDictionary<byte, IHandler> handlers)
        {
            _args = args;
            _handlers = handlers;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var udp = new UdpClient(0);
            var serverEp = new IPEndPoint(IPAddress.Parse(_args["server"]), int.Parse(_args["port"]));

            // REGISTER
            await udp.SendAsync(new[] { Command.REGISTER }, 1, serverEp);

            // Receive IDnew
            var result = await udp.ReceiveAsync();
            var resp = result.Buffer;

            if (resp.Length != 11 || resp[0] != (byte)Command.ID)
            {
                Console.WriteLine("[Error] Registration failed.");
                return;
            }

            var serverId = Encoding.UTF8.GetString(resp, 1, 10);
            Console.WriteLine($"[Info] Registered ID: {serverId}");

            // Start listening for control and streaming
            var receiveControlLoopTask = Task.Run(() => ReceiveControlLoop(udp, serverId));
            var captureLoopTask = Task.Run(() => CaptureLoop(udp, serverEp, serverId));

        }

        async void CaptureLoop(UdpClient udp, IPEndPoint intermediaryEp, string serverId)
        {
            var frameId = 0;
            var fps = int.Parse(_args["fps"]);
            var delay = 60000 / fps;

            while (true)
            {
                var info = new CursorInfo();
                info.cbSize = Marshal.SizeOf(info);

                if (Win32.GetCursorInfo(out info) && info.flags == Win32.CURSOR_SHOWING)
                {
                    //TODO:implement cursor shape sharing
                    //info.hCursor -> contains the cursor shape
                }

                var data = ScreenCapturerWin32Impl.CaptureScreenBytes();
                var maxPacketSize = 60 * 1024; // güvenli limit (~60 KB)
                var offset = 0;
                var index = 0;
                var totalPackets = data.Length / maxPacketSize;

                using var ms = new MemoryStream();
                using var writer = new BinaryWriter(ms, Encoding.UTF8);

                while (offset < data.Length)
                {
                    var chunkSize = Math.Min(maxPacketSize, data.Length - offset);

                    writer.Write(Command.FRAME, serverId, frameId, totalPackets, index);
                    writer.Write(data, offset, chunkSize);
                    writer.Flush();

                    var chunk = ms.GetBuffer();

                    ms.SetLength(0);
                    ms.Position = 0;

                    await udp.SendAsync(chunk, chunk.Length, intermediaryEp);

                    offset += chunkSize;
                    index++;
                }

                await Task.Delay(delay);
                frameId++;
            }
        }
        async Task ReceiveControlLoop(UdpClient udp, string serverId)
        {
            while (true)
            {
                try
                {
                    var result = await udp.ReceiveAsync();
                    var data = result.Buffer;
                    var command = data[0];
                    var action = data[1];

                    if (command != (byte)Command.CONTROL)
                        continue;

                    _handlers[action].Handle(udp, result);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }
}
