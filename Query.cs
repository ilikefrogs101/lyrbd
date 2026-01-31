using System.Text;
using ilikefrogs101.Logging;

namespace ilikefrogs101.MusicPlayer;
public static class Query {
    public static void Enquire(string mode, string type, string source) {
        switch (mode) {
            case "list":
                _list(type, source);
                break;
            case "lookup":
                _lookup(type, source);
                break;
        }
    }

    private static void _list(string type, string source) {

    }
    private static void _lookup(string type, string source) {

        if (source == "current") {
            source = AudioHandler.GetCurrentTrackId();
        }

        switch (type) {
            case "source":
                _source();
                break;
            case "queue":
                _queue();
                break;
            case "queueposition":
                _queuePosition();
                break;
            case "title":
                _trackTitle(source);
                break;
            case "artists":
                _trackArtists(source);
                break;
            case "album":
                _trackAlbum(source);
                break;
        }
    }

    private static void _source() {
        Log.OutputResponse(AudioHandler.GetSource());
    }
    public static void _trackTitle(string track) {
        Log.OutputResponse(TrackManager.GetTrack(track).Title);
    }
    public static void _trackArtists(string track) {
        Log.OutputResponse(string.Join(',', TrackManager.GetTrack(track).Artists));
    }
    public static void _trackAlbum(string track){
        Log.OutputResponse(TrackManager.GetTrack(track).Album);
    }
    public static void _currentTrackProgress() {
        Log.OutputResponse(AudioHandler.GetProgress().ToString());
    }
    public static void _currentTrackLengthlength() {
        Log.OutputResponse(AudioHandler.GetLength().ToString());
    }
    public static void _queue() {
        StringBuilder output = new();

        string[] queue = AudioHandler.GetQueue();
        for(int i = 0; i < queue.Length; ++i) {
            output.Append(i);
            output.Append(". ");
            output.Append(TrackManager.GetTrack(queue[i]).Title);
            output.Append('\n');
        }

        Log.OutputResponse(output.ToString());
    }
    public static void _queuePosition() {
        Log.OutputResponse(AudioHandler.GetQueuePosition().ToString());
    }
}