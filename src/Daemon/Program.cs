using System;
using ilikefrogs101.CLI;
using ilikefrogs101.Logging;
using ilikefrogs101.ICP;

namespace Lyrbd.Daemon;
public static class Program {
    public static void Main() {
        IcpServer server = new IcpServer();
        Log.OnResponse += server.Broadcast;
        Log.OnDebugInformation += Console.WriteLine;
        Log.OnErrorInformation += Console.WriteLine;

        CommandRegistry.LoadCommands(typeof(Commands));
        TrackManager.Initialise();

        server.MessageReceived += CommandParser.ParseCommand;
        server.Listen("/tmp/lyrbd.sock");
    }
}