using ilikefrogs101.CommandHandler;
using ilikefrogs101.Logging;

namespace ilikefrogs101.MusicPlayer;
public static class Program {
    public static void Main() {
        Log.Response += IcpHandler.BroadcastResponse;

        CommandRegistry.LoadCommands(typeof(Commands));

        IcpHandler.CommandReceived += CommandParser.ParseCommand;
        IcpHandler.ListenForCommands();

    }
}