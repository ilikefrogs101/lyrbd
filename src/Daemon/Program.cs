using ilikefrogs101.CLI;
using ilikefrogs101.Logging;

namespace ilikefrogs101.MusicPlayer;
public static class Program {
    public static void Main() {
        Log.OnResponse += IpcHandler.BroadcastResponse;
        Log.OnDebugInformation += Console.WriteLine;
        Log.OnErrorInformation += Console.WriteLine;

        CommandRegistry.LoadCommands(typeof(Commands));
        TrackManager.Initialise();

        IpcHandler.CommandReceived += CommandParser.ParseCommand;
        IpcHandler.ListenForCommands();
    }
}