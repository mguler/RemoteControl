using RemoteControl.IntermediateServer;
using RemoteControl.IntermediateServer.CommandHandlers;
using RemoteControl.Shared;

const int port = 7000;
var server = new ServerImpl();
server.Handlers[Command.REGISTER] = new Register(server);
server.Handlers[Command.SUBSCRIBE] = new Subscribe(server);
server.Handlers[Command.FRAME] = new Frame(server);
server.Handlers[Command.CONTROL] = new Control(server);

Console.WriteLine($"[Info] UDP intermediary listening on port {port}...");

await server.Start(port);
