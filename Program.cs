using ilikefrogs101.CommandHandler;
using ilikefrogs101.Logging;

namespace ilikefrogs101.MusicPlayer;
public static class Program {
    public static void Main() {
        Log.OnResponse += IcpHandler.BroadcastResponse;
        Log.OnDebugInformation += Console.WriteLine;
        Log.OnErrorInformation += Console.WriteLine;

        CommandRegistry.LoadCommands(typeof(Commands));

        TrackManager.Initialise();

        IcpHandler.CommandReceived += CommandParser.ParseCommand;
        IcpHandler.ListenForCommands();
    }
}