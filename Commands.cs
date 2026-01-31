using ilikefrogs101.CommandHandler;
using ilikefrogs101.Shutdown;

namespace ilikefrogs101.MusicPlayer;
public static class Commands {
    [Command(Name = "quit", Description = "kill the background process")]
    public static void Quit(Dictionary<string, string> arguments) {
        ShutdownHandler.RequestShutdown();
    }

    [Command(Name = "pause", Description = "pause the current track")]
    public static void Pause(Dictionary<string, string> arguments) {
        AudioHandler.Pause();
    }
    [Command(Name = "resume", Description = "resume playing the current track")]
    public static void Resume(Dictionary<string, string> arguments) {
        AudioHandler.Resume();
    }
    [Command(Name = "restart", Description = "restart the current track")]
    public static void Restart(Dictionary<string, string> arguments) {
        AudioHandler.Restart();
    }
    [Command(Name = "stop", Description = "stop playing the current track")]
    public static void Stop(Dictionary<string, string> arguments) {
        AudioHandler.Stop();
    }
    [Command(Name = "next", Description = "advance to the next track in the queue")]
    public static void Next(Dictionary<string, string> arguments) {
        AudioHandler.Next();
    }
    [Command(Name = "previous", Description = "revert to the previous track in the queue")]
    public static void Previous(Dictionary<string, string> arguments) {
        AudioHandler.Previous();
    }
    [Command(Name = "play", Description = "play a track, playlist, album, or artist")]
    [Argument(Name = "id", ArgumentType = ArgumentType.PositionalRequired)]
    public static void Play(Dictionary<string, string> arguments) {
        string id = arguments["id"];
        AudioHandler.Play(id);
    }
    [Command(Name = "forward", Description = "skip forwards in the current track")]
    [Argument(Name = "seconds", ArgumentType = ArgumentType.PositionalRequired)]
    public static void Forward(Dictionary<string, string> arguments) {
        ulong seconds = Convert.ToUInt64(arguments["seconds"]);
        AudioHandler.Forward(seconds);
    }
    [Command(Name = "backward", Description = "skip backwards in the current track")]
    [Argument(Name = "seconds", ArgumentType = ArgumentType.PositionalRequired)]
    public static void Backward(Dictionary<string, string> arguments) {
        ulong seconds = Convert.ToUInt64(arguments["seconds"]);
        AudioHandler.Backward(seconds);
    }
    [Command(Name = "import", Description = "import a track to be played")]
    [Argument(Name = "path", ArgumentType = ArgumentType.PositionalRequired)]
    [Argument(Name = "title", ArgumentType = ArgumentType.Flag)]
    [Argument(Name = "artists", ArgumentType = ArgumentType.Flag)]
    [Argument(Name = "album", ArgumentType = ArgumentType.Flag)]
    [Argument(Name = "tracknumber", ArgumentType = ArgumentType.Flag)]
    public static void Import(Dictionary<string, string> arguments) {
        if(arguments.ContainsKey("title"))
            FileHandler.TitleOverride = arguments["title"];

        if(arguments.ContainsKey("artists"))
            FileHandler.ArtistsOverride = arguments["artists"].Split(',');

        if(arguments.ContainsKey("album"))
            FileHandler.AlbumOverride = arguments["album"];

        if(arguments.ContainsKey("tracknumber"))
            FileHandler.TrackNumberOverride = Convert.ToUInt32(arguments["tracknumber"]);

        string path = arguments["path"];
        FileHandler.ImportTrack(path);
    }
    public static void Export(Dictionary<string, string> arguments) {

    }
    [Command(Name = "query", Description = "fetch information from the music player")]
    [Argument(Name = "type", ArgumentType = ArgumentType.PositionalRequired)]
    [Argument(Name = "source", ArgumentType = ArgumentType.PositionalOptional)]
    public static void Query(Dictionary<string, string> arguments) {
        string type = arguments["type"];
        string source = null;

        if(arguments.ContainsKey("source"))
            source = arguments["source"];

        MusicPlayer.Query.Enquire(type, source);
    }
    public static void Playlist(Dictionary<string, string> arguments) {

    }
    [Command(Name = "shuffle", Description = "change the shuffle setting")]
    [Argument(Name = "on/off", ArgumentType = ArgumentType.PositionalRequired)]
    public static void Shuffle(Dictionary<string, string> arguments) {

    }
    [Command(Name = "loop", Description = "change the loop setting")]
    [Argument(Name = "on/off", ArgumentType = ArgumentType.PositionalRequired)]
    public static void Loop(Dictionary<string, string> arguments) {

    }
    [Command(Name = "volume", Description = "change the volume setting")]
    [Argument(Name = "percent", ArgumentType = ArgumentType.PositionalRequired)]
    public static void Volume(Dictionary<string, string> arguments) {

    }
}