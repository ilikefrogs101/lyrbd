using System.Data.Common;
using System.Text;
using ilikefrogs101.Logging;

namespace Lyrbd.Daemon;
public static class Query {
    private static AddressType _lastAddressQueryType;
    private static string[] _lastAddressQueryOutput;
    public static string AddressFromIndex(int index) {
        if (index > _lastAddressQueryOutput.Length || index < 0) return default;

        return $"{_lastAddressQueryType}:{_lastAddressQueryOutput[index]}";
    }
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

        if (output != default) Log.OutputResponse(output);
    }

    private static string _lookupQueue() {
        _lastAddressQueryType = AddressType.TRACK;
        _lastAddressQueryOutput = QueueHandler.Queue();

        return _buildAddressQueryOutput();
    }
    private static string _lookupTracks(string address) {
        _lastAddressQueryType = AddressType.TRACK;

        if(address == default) {
            _lastAddressQueryOutput = LibraryManager.Tracks();
            return _buildAddressQueryOutput();
        }

        _lastAddressQueryOutput = LibraryManager.TracksFromAddress(address);
        return _buildAddressQueryOutput();
    }
    private static string _lookupPlaylists() {
        _lastAddressQueryType = AddressType.PLAYLIST;

        _lastAddressQueryOutput = LibraryManager.Playlists();
        return _buildAddressQueryOutput();
    }
    private static string _lookupArtists(string address) {
        _lastAddressQueryType = AddressType.ARTIST;

        if(address == default) {
            _lastAddressQueryOutput = LibraryManager.Artists();
            return _buildAddressQueryOutput();
        }

        (AddressType type, string id) = LibraryManager.ParseAddress(address);
        switch (type) {
            case AddressType.TRACK:
                _lastAddressQueryOutput = LibraryManager.Track(id).Artists;
                break;
            case AddressType.ALBUM:
                _lastAddressQueryOutput = [.. LibraryManager.Album(id).Artists];
                break;
        }

        return _buildAddressQueryOutput();
    }
    private static string _lookupAlbums(string address) {
        _lastAddressQueryType = AddressType.ALBUM;

        if(address == default) {
            _lastAddressQueryOutput = LibraryManager.Albums();
            return _buildAddressQueryOutput();
        }

        (AddressType type, string id) = LibraryManager.ParseAddress(address);
        switch (type) {
            case AddressType.TRACK:
                _lastAddressQueryOutput = [LibraryManager.Track(id).Album];
                break;
            case AddressType.ARTIST:
                _lastAddressQueryOutput = [.. LibraryManager.Artist(id).Albums];
                break;
        }

        return _buildAddressQueryOutput();
    }
    private static string _buildAddressQueryOutput() {
        if (_lastAddressQueryOutput == null || _lastAddressQueryOutput.Length < 1) return default;

        StringBuilder output = new();
        for(int i = 0; i < _lastAddressQueryOutput.Length; ++i) {
            output.Append(i);
            output.Append(". ");
            output.Append(_lastAddressQueryOutput[i]);
            output.Append('\n');
        }

        return output.ToString();
    }
}