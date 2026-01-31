using ilikefrogs101.CommandHandler;
namespace ilikefrogs101.MusicPlayer;
public static class Commands {
    [CommandAttribute(Name = "quit", Description = "Kill the process")]
    public static void Quit(Dictionary<string, object> arguments) {
        Console.WriteLine("Quitting...");
        Environment.Exit(0);
    }
    public static void Pause(Dictionary<string, object> arguments) {

    }
    public static void Resume(Dictionary<string, object> arguments) {

    }
    public static void Restart(Dictionary<string, object> arguments) {

    }
    public static void Stop(Dictionary<string, object> arguments) {

    }
    public static void Next(Dictionary<string, object> arguments) {

    }
    public static void Previous(Dictionary<string, object> arguments) {

    }
    public static void Play(Dictionary<string, object> arguments) {

    }
    public static void Forward(Dictionary<string, object> arguments) {

    }
    public static void Backward(Dictionary<string, object> arguments) {

    }
    public static void Import(Dictionary<string, object> arguments) {

    }
    public static void Export(Dictionary<string, object> arguments) {

    }
    public static void Query(Dictionary<string, object> arguments) {

    }
    public static void Playlist(Dictionary<string, object> arguments) {

    }
    public static void Shuffle(Dictionary<string, object> arguments) {

    }
    public static void Loop(Dictionary<string, object> arguments) {

    }
    public static void Volume(Dictionary<string, object> arguments) {

    }
}