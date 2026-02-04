using System;
using System.Text;
using ilikefrogs101.IPC;
using ilikefrogs101.Logging;

namespace Lyrbd.Controller;
public static class Program {
    public static void Main() {
        Log.OnResponse += Console.WriteLine;

        IpcClient client = new IpcClient();
        client.Connect("/tmp/lyrbd.sock");
        
        string rawCommand = Environment.CommandLine.Split(' ', 2)[1];
        client.Send(rawCommand);

        Log.OutputResponse(client.Receive(16384));
    }
}