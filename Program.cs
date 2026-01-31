using ilikefrogs101.CommandHandler;

namespace ilikefrogs101.MusicPlayer;
public static class Program {
    public static void Main() {
        CommandRegistry.LoadCommands(typeof(Commands));

        IcpHandler.CommandReceived += CommandParser.ParseCommand;
        IcpHandler.ListenForCommands();
    }
}