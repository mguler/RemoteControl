using RemoteControl.CaptureService;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Net;
using System.Text;
using RemoteControl.Shared;
using RemoteControl.Shared.Extensions;
using RemoteControl.CaptureService.CommandHandlers;
using System.IO;

static class Program
{
    private const string IntermediaryIp = "192.168.1.42";
    private const int IntermediaryPort = 7000;
    private const int CaptureIntervalMs = 50;

    public static ConcurrentDictionary<byte, IHandler> Handlers { get; private set; } = new ConcurrentDictionary<byte, IHandler>();

    static Program()
    {
        Handlers[ControlCommand.MOUSE_MOVE] = new MouseMoveHandler();
        Handlers[ControlCommand.MOUSE_DOWN] = new MouseDownHandler();
        Handlers[ControlCommand.MOUSE_UP] = new MouseUpHandler();
        Handlers[ControlCommand.KEY_DOWN] = new KeyDownHandler();
        Handlers[ControlCommand.KEY_UP] = new KeyUpHandler();
    }

    static async Task Main()
    {
        Win32.AllocConsole();
        Thread.Sleep(100);
        using var udp = new UdpClient(0);
        var intermediaryEp = new IPEndPoint(IPAddress.Parse(IntermediaryIp), IntermediaryPort);

        // REGISTER
        await udp.SendAsync(new[] { (byte)Command.REGISTER }, 1, intermediaryEp);

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

        try
        {
            // Start listening for control and streaming
            _ = Task.Run(() => ReceiveControlLoop(udp, serverId));
            await CaptureLoop(udp, intermediaryEp, serverId);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

        Win32.FreeConsole();
    }

    static async Task CaptureLoop(UdpClient udp, IPEndPoint intermediaryEp, string serverId)
    {
        var frameId = 0;
        while (true)
        {
            var data = ScreenCapture.CaptureScreenBytes();
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

            await Task.Delay(CaptureIntervalMs);
            frameId++;
        }
    }
    static async Task ReceiveControlLoop(UdpClient udp, string serverId)
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

                Handlers[action].Handle(udp, result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}