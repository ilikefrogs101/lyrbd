using System;
using ilikefrogs101.CLI;
using ilikefrogs101.Logging;
using ilikefrogs101.IPC;

namespace Lyrbd.Daemon;
public static class Program {
    public static void Main() {
        Log.OnDebugInformation += Console.WriteLine;
        Log.OnErrorInformation += Console.WriteLine;

        CommandRegistry.LoadCommands(typeof(Commands));
        TrackManager.Initialise();
        AudioHandler.Initialise();

        IpcServer server = new IpcServer();
        Log.OnResponse += server.Broadcast;
        server.MessageReceived += CommandParser.ParseCommand;
        server.Listen("/tmp/lyrbd.sock");
    }
}