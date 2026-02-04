using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using ilikefrogs101.CommandHandler;
using ilikefrogs101.Shutdown;

namespace ilikefrogs101.MusicPlayer;
public static class Commands {
    [Command(Name = "quit", Description = "kill the background process")]
    public static void Quit(Arguments arguments) {
        ShutdownHandler.RequestShutdown();
    }

    [Command(Name = "pause", Description = "pause the current track")]
    public static void Pause(Arguments arguments) {
        AudioHandler.Pause();
    }
    [Command(Name = "resume", Description = "resume playing the current track")]
    public static void Resume(Arguments arguments) {
        AudioHandler.Resume();
    }
    [Command(Name = "restart", Description = "restart the current track")]
    public static void Restart(Arguments arguments) {
        AudioHandler.Restart();
    }
    [Command(Name = "stop", Description = "stop playing the current track")]
    public static void Stop(Arguments arguments) {
        AudioHandler.Stop();
    }
    [Command(Name = "next", Description = "advance to the next track in the queue")]
    public static void Next(Arguments arguments) {
        AudioHandler.Next();
    }
    [Command(Name = "previous", Description = "revert to the previous track in the queue")]
    public static void Previous(Arguments arguments) {
        AudioHandler.Previous();
    }
    [Command(Name = "skipqueue", Description = "skip to a position in the current queue")]
    [Argument(Name = "position", ArgumentType = ArgumentType.PositionalRequired)]
    public static void SkipQueue(Arguments arguments) {
        arguments.GetArgumentValue("position", out int position);
        AudioHandler.SkipQueue(position);
    }
    [Command(Name = "play", Description = "play a track, playlist, album, or artist")]
    [Argument(Name = "id", ArgumentType = ArgumentType.PositionalRequired)]
    public static void Play(Arguments arguments) {
        arguments.GetArgumentValue("id", out string id);
        AudioHandler.Play(id);
    }
    [Command(Name = "forward", Description = "skip forwards in the current track")]
    [Argument(Name = "seconds", ArgumentType = ArgumentType.PositionalRequired)]
    public static void Forward(Arguments arguments) {
        arguments.GetArgumentValue("seconds", out ulong seconds);
        AudioHandler.Forward(seconds);
    }
    [Command(Name = "backward", Description = "skip backwards in the current track")]
    [Argument(Name = "seconds", ArgumentType = ArgumentType.PositionalRequired)]
    public static void Backward(Arguments arguments) {
        arguments.GetArgumentValue("seconds", out ulong seconds);
        AudioHandler.Backward(seconds);
    }
    [Command(Name = "import", Description = "import a track to be played")]
    [Argument(Name = "path", ArgumentType = ArgumentType.PositionalRequired)]
    [Argument(Name = "titleoverride", ArgumentType = ArgumentType.Flag)]
    [Argument(Name = "artistsoverride", ArgumentType = ArgumentType.Flag)]
    [Argument(Name = "albumoverride", ArgumentType = ArgumentType.Flag)]
    [Argument(Name = "tracknumberoverride", ArgumentType = ArgumentType.Flag)]
    [Argument(Name = "autospace", ArgumentType = ArgumentType.Flag, Boolean = true)]
    [Argument(Name = "autocapitalise", ArgumentType = ArgumentType.Flag, Boolean = true)]
    [Argument(Name = "stripnumbers", ArgumentType = ArgumentType.Flag, Boolean = true)]
    [Argument(Name = "strippunctuation", ArgumentType = ArgumentType.Flag, Boolean = true)]
    public static void Import(Arguments arguments) {
        arguments.GetArgumentValue("titleoverride", out FileHandler.TitleOverride);
        arguments.GetArgumentValue("albumoverride", out FileHandler.TitleOverride);
        arguments.GetArgumentValue("tracknumberoverride", out FileHandler.TrackNumberOverride);

        arguments.GetArgumentValue("artistsoverride", out string artists);
        FileHandler.ArtistsOverride = artists.Split(',');

        FileHandler.AutoSpace = arguments.FlagTrigged("autospace");
        FileHandler.AutoCapitalise = arguments.FlagTrigged("autocapitalise");
        FileHandler.StripNumbers = arguments.FlagTrigged("stripnumbers");
        FileHandler.StripPunctuation = arguments.FlagTrigged("strippunctuation");

        arguments.GetArgumentValue("path", out string path);
        FileHandler.Import(path);
    }
    public static void Export(Arguments arguments) {

    }
    [Command(Name = "delete", Description = "delete a track")]
    [Argument(Name = "id", ArgumentType = ArgumentType.PositionalRequired)]
    public static void Delete(Arguments arguments) {
        arguments.GetArgumentValue("id", out string id);
        TrackManager.DeleteTrack(id);
    }
    [Command(Name = "query", Description = "fetch information from the music player")]
    [Argument(Name = "type", ArgumentType = ArgumentType.PositionalRequired)]
    [Argument(Name = "source", ArgumentType = ArgumentType.PositionalOptional)]
    public static void Query(Arguments arguments) {
        arguments.GetArgumentValue("type", out string type);
        arguments.GetArgumentValue("source", out string source);
        MusicPlayer.Query.Enquire(type, source);
    }
    [Command(Name = "playlist", Description = "modify playlists")]
    [Argument(Name = "playlist", ArgumentType = ArgumentType.PositionalRequired)]
    [Argument(Name = "mode", ArgumentType = ArgumentType.PositionalRequired)]
    [Argument(Name = "id", ArgumentType = ArgumentType.PositionalOptional)]
    public static void Playlist(Arguments arguments) {
        arguments.GetArgumentValue("playlist", out string playlist);
        arguments.GetArgumentValue("mode", out string mode);
        arguments.GetArgumentValue("id", out string id);

        switch (mode) {
            case "add":
                TrackManager.AddToPlaylist(playlist, id);
                break;
            case "remove":
                TrackManager.RemoveFromPlaylist(playlist, id);
                break;
            case "delete":
                TrackManager.DeletePlaylist(playlist);
                break;
        }
    }
    [Command(Name = "shuffle", Description = "change the shuffle setting")]
    [Argument(Name = "on/off", ArgumentType = ArgumentType.PositionalRequired)]
    public static void Shuffle(Arguments arguments) {
        arguments.GetArgumentValue("on/off", out string state);
        if(state == "on") {
            AudioHandler.SetShuffle(true);
        }
        if(state == "off") {
            AudioHandler.SetShuffle(false);
        }
    }
    [Command(Name = "loop", Description = "change the loop setting")]
    [Argument(Name = "on/off", ArgumentType = ArgumentType.PositionalRequired)]
    public static void Loop(Arguments arguments) {
        arguments.GetArgumentValue("on/off", out string state);
        if(state == "on") {
            AudioHandler.SetLoop(true);
        }
        if(state == "off") {
            AudioHandler.SetLoop(false);
        }
    }
    [Command(Name = "volume", Description = "change the volume setting")]
    [Argument(Name = "percent", ArgumentType = ArgumentType.PositionalRequired)]
    public static void Volume(Arguments arguments) {
        arguments.GetArgumentValue("percent", out string percent);
        percent = new string([.. percent.Where(char.IsDigit)]);

        float volume = (float)Convert.ToDouble(percent);
        AudioHandler.SetVolume(volume);
    }
}