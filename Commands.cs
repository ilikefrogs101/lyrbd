using System.Runtime.Serialization;
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
    [Command(Name = "skipqueue", Description = "skip to a position in the current queue")]
    [Argument(Name = "position", ArgumentType = ArgumentType.PositionalRequired)]
    public static void SkipQueue(Dictionary<string, string> arguments) {
        int position = Convert.ToInt32(arguments["position"]);
        AudioHandler.SkipQueue(position);
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
    [Argument(Name = "titleoverride", ArgumentType = ArgumentType.Flag)]
    [Argument(Name = "artistsoverride", ArgumentType = ArgumentType.Flag)]
    [Argument(Name = "albumoverride", ArgumentType = ArgumentType.Flag)]
    [Argument(Name = "tracknumberoverride", ArgumentType = ArgumentType.Flag)]
    [Argument(Name = "spacebeforecapitals", ArgumentType = ArgumentType.Flag, Boolean = true)]
    [Argument(Name = "underscoremeansspace", ArgumentType = ArgumentType.Flag, Boolean = true)]
    [Argument(Name = "hyphenmeansspace", ArgumentType = ArgumentType.Flag, Boolean = true)]
    [Argument(Name = "autocapitalise", ArgumentType = ArgumentType.Flag, Boolean = true)]
    [Argument(Name = "filtercharacters", ArgumentType = ArgumentType.Flag, Boolean = true)]
    [Argument(Name = "stripnumbers", ArgumentType = ArgumentType.Flag, Boolean = true)]
    public static void Import(Dictionary<string, string> arguments) {
        if(arguments.ContainsKey("titleoverride"))
            FileHandler.TitleOverride = arguments["titleoverride"];

        if(arguments.ContainsKey("artistsoverride"))
            FileHandler.ArtistsOverride = arguments["artistsoverride"].Split(',');

        if(arguments.ContainsKey("albumoverride"))
            FileHandler.AlbumOverride = arguments["albumoverride"];

        if(arguments.ContainsKey("tracknumberoverride"))
            FileHandler.TrackNumberOverride = Convert.ToUInt32(arguments["tracknumberoverride"]);

        if(arguments.ContainsKey("spacebeforecapitals"))
            FileHandler.SpaceBeforeCapitals = true;
        
        if(arguments.ContainsKey("underscoremeansspace")) 
            FileHandler.UnderscoreMeansSpace = true;
        
        if(arguments.ContainsKey("hyphenmeansspace")) 
            FileHandler.HyphenMeansSpace = true;

        if(arguments.ContainsKey("autocapitalise"))
            FileHandler.AutoCapitalise = true;

        if(arguments.ContainsKey("filtercharacters"))
            FileHandler.FilterCharacters = true;

        if(arguments.ContainsKey("stripnumbers"))
            FileHandler.StripNumbers = true;

        string path = arguments["path"];
        FileHandler.Import(path);
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
    [Command(Name = "playlist", Description = "modify playlists")]
    [Argument(Name = "playlist", ArgumentType = ArgumentType.PositionalRequired)]
    [Argument(Name = "mode", ArgumentType = ArgumentType.PositionalRequired)]
    [Argument(Name = "id", ArgumentType = ArgumentType.PositionalOptional)]
    public static void Playlist(Dictionary<string, string> arguments) {
        string playlist = arguments["playlist"];
        string mode = arguments["mode"];
        string id = null;

        if(arguments.ContainsKey("id"))
            id = arguments["id"];

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
    public static void Shuffle(Dictionary<string, string> arguments) {
        string state = arguments["on/off"];
        if(state == "on") {
            AudioHandler.SetShuffle(true);
        }
        if(state == "off") {
            AudioHandler.SetShuffle(false);
        }
    }
    [Command(Name = "loop", Description = "change the loop setting")]
    [Argument(Name = "on/off", ArgumentType = ArgumentType.PositionalRequired)]
    public static void Loop(Dictionary<string, string> arguments) {
        string state = arguments["on/off"];
        if(state == "on") {
            AudioHandler.SetLoop(true);
        }
        if(state == "off") {
            AudioHandler.SetLoop(false);
        }
    }
    [Command(Name = "volume", Description = "change the volume setting")]
    [Argument(Name = "percent", ArgumentType = ArgumentType.PositionalRequired)]
    public static void Volume(Dictionary<string, string> arguments) {
        string percent = arguments["percent"];
        percent = new string([.. percent.Where(char.IsDigit)]);
        float volume = (float)Convert.ToDouble(percent);
        AudioHandler.SetVolume(volume);
    }
}