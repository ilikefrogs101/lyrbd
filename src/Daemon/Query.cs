using System.Data.Common;
using System.Text;
using ilikefrogs101.Logging;

namespace Lyrbd.Daemon;
public static class Query {
    public static void Enquire(string type, string address) {
        string output = default;

        if (address == "current") {
            address = QueueHandler.TrackId();
        }

        switch (type) {
            case "current":
                output = QueueHandler.TrackId();
                break;
            case "address":
                output = QueueHandler.Address();
                break;
            case "queueposition":
                output = QueueHandler.QueuePosition().ToString();
                break;
            case "queue":
                output = _lookupQueue();
                break;
            case "progress":
                output = AudioHandler.TrackProgress().ToString();
                break;
            case "length":
                output = AudioHandler.TrackLength().ToString();
                break;
            case "pause":
                output = AudioHandler.IsPaused().ToString();
                break;
            case "loop":
                output = QueueHandler.IsLooped().ToString();
                break;
            case "shuffle":
                output = QueueHandler.IsShuffled().ToString();
                break;
            case "volume":
                output = (AudioHandler.Volume() * 100).ToString();
                break;
            case "tracks":
                output = _lookupTracks(address);
                break;
            case "playlists":
                output = _lookupPlaylists();
                break;
            case "artists":
                output = _lookupArtists(address);
                break;
            case "albums":
                output = _lookupAlbums(address);
                break;
        }

        Log.OutputResponse(output);
    }

    private static string _lookupQueue() {
        StringBuilder output = new();

        string[] queue = QueueHandler.Queue();
        for(int i = 0; i < queue.Length; ++i) {
            output.Append(i);
            output.Append(". ");
            output.Append(queue[i]);
            output.Append('\n');
        }

        return output.ToString();
    }
    private static string _lookupTracks(string address) {
        if(address == default)
            return string.Join('\n', LibraryManager.Tracks());
        
        return string.Join('\n', LibraryManager.TracksFromAddress(address));
    }
    private static string _lookupPlaylists() {
        return string.Join('\n', LibraryManager.Playlists());
    }
    private static string _lookupArtists(string address) {
        if(address == default) {
            string[] artists = LibraryManager.Artists();
            return string.Join('\n', artists);
        }

        (string type, string id) = LibraryManager.SplitAddress(address);
        switch (type) {
            case "track":
                return LibraryManager.Track(id).Album;
            case "artist":
                string[] albums = [.. LibraryManager.Artist(id).Albums];
                return string.Join('\n', albums);
        }

        return null;
    }
    private static string _lookupAlbums(string address) {
        if(address == default) {
            string[] albums = LibraryManager.Albums();
            return string.Join('\n', albums);
        }

        (string type, string id) = LibraryManager.SplitAddress(address);
        switch (type) {
            case "track":
                return string.Join('\n', LibraryManager.Track(id).Artists);
            case "album":
                string[] artists = [.. LibraryManager.Album(id).Artists];
                return string.Join('\n', artists);
        }

        return null;
    }
}