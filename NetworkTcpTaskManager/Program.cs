using NetworkTcpTaskManager;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;

var ip = IPAddress.Loopback;
var port = 4444;

var listener = new TcpListener(ip, port);

listener.Start();


Console.WriteLine("Server started");
Thread.Sleep(1000);
Console.WriteLine($"Server login from: {ip} | {port}");
Thread.Sleep(1000);
Console.WriteLine("Listening...\n");



while (true)
{
    var client = listener.AcceptTcpClient();
    var stream = client.GetStream();
    var br = new BinaryReader(stream);
    var bw = new BinaryWriter(stream);


    while (true)
    {
        var input = br.ReadString();
        var command = JsonSerializer.Deserialize<Command>(input);

        if (command is null) continue;
        Console.WriteLine(command.Text);
        Console.WriteLine(command.Param);
        switch (command.Text)
        {
            case Command.ProcessList:
                var processes = Process.GetProcesses();
                var processesNames = JsonSerializer.Serialize(processes.Select(p => p.ProcessName));
                bw.Write(processesNames);
                Console.WriteLine($"Process List Show - {DateTime.Now}");
                break;

            case Command.Run:
                string processName = command.Param!;
                Process.Start(processName);
                Console.WriteLine($"New task run - {DateTime.Now}");
                break;

            case Command.Kill:
                processName = command.Param!;
                Process process = Process.GetProcessesByName(processName)[0];
                process.Kill();
                Console.WriteLine($"Any task has kill - {DateTime.Now}");
                break;

            default:
                break;
        }
    }
}