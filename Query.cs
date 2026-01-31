using System.ComponentModel;
using System.Text;
using ilikefrogs101.Logging;

namespace ilikefrogs101.MusicPlayer;
public static class Query {
    public static void Enquire(string type, string source) {
        if (source == "current") {
            source = AudioHandler.GetCurrentTrackId();
        }

        switch (type) {
            case "current":
                _currentTrack();
                break;
            case "source":
                _source();
                break;
            case "queue":
                _queue();
                break;
            case "queueposition":
                _queuePosition();
                break;
            case "progress":
                _currentTrackProgress();
                break;
            case "length":
                _currentTracklength();
                break;
            case "tracks":
                _lookupTracks(source);
                break;
            case "artists":
                _lookupTracks(source);
                break;
            case "albums":
                _lookupTracks(source);
                break;
        }
    }

    private static void _lookupTracks(string source) {
        string idType = source.Split(':')[0];
        string id = source.Split(':')[1];


        switch (idType) {
            case "artist":
                string[] tracks = [.. TrackManager.GetArtist(id).Tracks];
                Log.OutputResponse(string.Join('\n', tracks));
                break;
            case "album":
                tracks = [.. TrackManager.GetAlbum(id).Tracks];
                Log.OutputResponse(string.Join('\n', tracks));
                break;
        }
    }
    private static void _lookupArtists(string source) {
        string idType = source.Split(':')[0];
        string id = source.Split(':')[1];

        switch (idType) {
            case "track":
                string[] artists = [.. TrackManager.GetTrack(id).Artists];
                Log.OutputResponse(string.Join('\n', artists));
                break;
            case "album":
                artists = [.. TrackManager.GetAlbum(id).Artists];
                Log.OutputResponse(string.Join('\n', artists));
                break;
        }
    }
    private static void _lookupAlbums(string source) {
        string idType = source.Split(':')[0];
        string id = source.Split(':')[1];

        switch (idType) {
            case "track":
                string album = TrackManager.GetTrack(id).Album;
                Log.OutputResponse(album);
                break;
            case "artist":
                string[] albums = [.. TrackManager.GetArtist(id).Albums];
                Log.OutputResponse(string.Join('\n', albums));
                break;
        }
    }
    private static void _currentTrack() {
        Log.OutputResponse(AudioHandler.GetCurrentTrackId());
    }
    private static void _queue() {
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
    private static void _queuePosition() {
        Log.OutputResponse(AudioHandler.GetQueuePosition().ToString());
    }
    private static void _source() {
        Log.OutputResponse(AudioHandler.GetSource());
    }
    private static void _currentTrackProgress() {
        Log.OutputResponse(AudioHandler.GetProgress().ToString());
    }
    private static void _currentTracklength() {
        Log.OutputResponse(AudioHandler.GetLength().ToString());
    }
}