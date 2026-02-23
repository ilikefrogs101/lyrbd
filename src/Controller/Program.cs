using System;
using System.Text;
using ilikefrogs101.IPC;
using ilikefrogs101.Logging;

namespace Lyrbd.Controller;
public static class Program {
    public const string SOCKET_PATH = "/tmp/lyrbd.sock";
    public static void Main() {
        Log.OnResponse += Console.WriteLine;

        if (File.Exists(SOCKET_PATH)) {
            IpcClient client = new IpcClient();
            client.Connect(SOCKET_PATH);
            
            string rawCommand = Environment.CommandLine.Split(' ', 2)[1];
            client.Send(rawCommand);

            Log.OutputResponse(client.Receive(4194304));
        }
        else {
            Log.OutputResponse("lyrbd not running");
        }
    }
}