using ilikefrogs101.CommandHandler;

namespace ilikefrogs101.MusicPlayer;
public static class Commands {
    [Command(Name = "quit", Description = "kill the background process")]
    public static void Quit(Dictionary<string, object> arguments) {
        Console.WriteLine("Quitting...");
        Environment.Exit(0);
    }

    [Command(Name = "pause", Description = "pause the current track")]
    public static void Pause(Dictionary<string, object> arguments) {

    }
    [Command(Name = "resume", Description = "resume playing the current track")]
    public static void Resume(Dictionary<string, object> arguments) {

    }
    [Command(Name = "restart", Description = "restart the current track")]
    public static void Restart(Dictionary<string, object> arguments) {

    }
    [Command(Name = "stop", Description = "stop playing the current track")]
    public static void Stop(Dictionary<string, object> arguments) {

    }
    [Command(Name = "next", Description = "advance to the next track in the queue")]
    public static void Next(Dictionary<string, object> arguments) {

    }
    [Command(Name = "previous", Description = "revert to the previous track in the queue")]
    public static void Previous(Dictionary<string, object> arguments) {

    }
    [Command(Name = "play", Description = "play a track, playlist, album, or artist")]
    [Argument(Name = "id", ArgumentType = ArgumentType.PositionalRequired)]
    public static void Play(Dictionary<string, object> arguments) {

    }
    [Command(Name = "forward", Description = "skip forwards in the current track")]
    [Argument(Name = "seconds", ArgumentType = ArgumentType.PositionalRequired)]
    public static void Forward(Dictionary<string, object> arguments) {

    }
    [Command(Name = "backward", Description = "skip backwards in the current track")]
    [Argument(Name = "seconds", ArgumentType = ArgumentType.PositionalRequired)]
    public static void Backward(Dictionary<string, object> arguments) {

    }
    [Command(Name = "import", Description = "import a track to be played")]
    [Argument(Name = "path", ArgumentType = ArgumentType.PositionalRequired)]
    [Argument(Name = "title", ArgumentType = ArgumentType.Flag)]
    [Argument(Name = "artists", ArgumentType = ArgumentType.Flag)]
    [Argument(Name = "album", ArgumentType = ArgumentType.Flag)]
    public static void Import(Dictionary<string, object> arguments) {

    }
    public static void Export(Dictionary<string, object> arguments) {

    }
    [Command(Name = "query", Description = "fetch information from the music player")]
    [Argument(Name = "type", ArgumentType = ArgumentType.PositionalRequired)]
    [Argument(Name = "location", ArgumentType = ArgumentType.PositionalOptional)]
    public static void Query(Dictionary<string, object> arguments) {

    }
    public static void Playlist(Dictionary<string, object> arguments) {

    }
    [Command(Name = "shuffle", Description = "change the shuffle setting")]
    [Argument(Name = "on/off", ArgumentType = ArgumentType.PositionalRequired)]
    public static void Shuffle(Dictionary<string, object> arguments) {

    }
    [Command(Name = "loop", Description = "change the loop setting")]
    [Argument(Name = "on/off", ArgumentType = ArgumentType.PositionalRequired)]
    public static void Loop(Dictionary<string, object> arguments) {

    }
    [Command(Name = "volume", Description = "change the volume setting")]
    [Argument(Name = "percent", ArgumentType = ArgumentType.PositionalRequired)]
    public static void Volume(Dictionary<string, object> arguments) {

    }
}