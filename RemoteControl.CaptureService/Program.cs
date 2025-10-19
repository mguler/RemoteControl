using RemoteControl.CaptureService;
using System.Collections.Concurrent;
using RemoteControl.Shared;
using RemoteControl.CaptureService.CommandHandlers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RemoteControl.Shared.Extensions;
using System.Diagnostics;

static class Program
{
    static async Task Main()
    {
        var args = CommandlineArguments.Get();
        var isServiceMode = !Environment.UserInteractive;
        var isHelpRequested = args.ContainsKey("help");

        if (isHelpRequested)
        {
            ShowHelp();
            return;
        }

        if (!CheckMandatoryParameters(args)) 
        {
            return;
        }

        var builder = Host.CreateDefaultBuilder()
        .ConfigureServices(services =>
        {
            services.AddSingleton(serviceResolver => args);
         
            services.AddSingleton(serviceResolver => {
                var handlers = new ConcurrentDictionary<byte, IHandler>();
                handlers[ControlCommand.MOUSE_MOVE] = new MouseMoveHandler();
                handlers[ControlCommand.MOUSE_DOWN] = new MouseDownHandler();
                handlers[ControlCommand.MOUSE_UP] = new MouseUpHandler();
                handlers[ControlCommand.KEY_DOWN] = new KeyDownHandler();
                handlers[ControlCommand.KEY_UP] = new KeyUpHandler();
                return handlers;
            });

            services.AddHostedService<RemoteControlService>();
        });

        if (isServiceMode)
        {
            #if DEBUG
            Debugger.Launch();
            #endif
             
            if (OperatingSystem.IsWindows())
            {
                builder.UseWindowsService();
            }
            else
            {
                Console.WriteLine("Application does not support installed OS in background mode");
            }
        }

        var host = builder.Build();
        await host.RunAsync();
    }

    static void ShowHelp()
    {
        Console.WriteLine($@"
Usage: RemoteControl.CaptureService.exe [option] <arguments> 

Remote control and screen view utility

Options:
  --ip                  Ip v4 address of RemoteControl server
  --port                Server port
  --fps                 Screen capture frequency (Frame Per Second) (10 , 15 , 20 , 25 are valid values)
  --console             Run in console mode
  --help                Show help and exit

Examples:
  RemoteControl.CaptureService.exe --ip 127.0.0.1 --port 7000 --fps 25
  RemoteControl.CaptureService.exe --ip 127.0.0.1 --port 7000 --fps 20 --console
                ");
    }
    static bool CheckMandatoryParameters(Dictionary<string,string> args) 
    {
        var result = true;

        if (args.ContainsKey("ip"))
        {
            if (!args["ip"].IsMatch("^(?:[1-9]\\d?|1\\d\\d|2[0-4]\\d|25[0-5])(?:\\.(?:0|[1-9]\\d?|1\\d\\d|2[0-4]\\d|25[0-5])){3}$"))
            {
                Console.WriteLine("Invalid IPv4 format. Expected: 0–255.0–255.0–255.0–255 (e.g. 192.168.1.100)");
                result = false;
            }
        }
        else
        {
            Console.WriteLine("Error: Missing required parameter. --ip");
            result = false;
        }

        if (args.ContainsKey("port"))
        {
            if (!args["port"].IsMatch("^(?:[1-9]|[1-9]\\d|[1-9]\\d{2}|[1-9]\\d{3}|[1-5]\\d{4}|6[0-4]\\d{3}|65[0-4]\\d{2}|655[0-2]\\d|6553[0-5])$"))
            {
                Console.WriteLine("Error: Invalid port number. Expected : An integer between: 1–65535.");
                result = false;
            }
        }
        else
        {
            Console.WriteLine("Error: Missing required parameter. --port");
            result = false;
        }

        if (args.ContainsKey("fps"))
        {
            if (!args["fps"].IsMatch("^10|15|20|25|30$"))
            {
                Console.WriteLine("Error: Wrong FPS value. Expected : valid values are 10,15,20,25,30 ");
                result = false;
            }
        }

        if (!result) 
        {
            Console.WriteLine("use --help command for more help");            
        }

        return result;
    }
}
