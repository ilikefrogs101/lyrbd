using System;
using System.Collections.Generic;
using System.Linq;
using F23.StringSimilarity;

namespace Lyrbd.Daemon;
public static class LibraryManager {
    private static Registry _registry;
    private static Dictionary<string, Artist> _artists;
    private static Dictionary<string, Album> _albums;
    
    private static Dictionary<string, string> _addressLookup;

    public static Track Track(string id) {
        if (int.TryParse(id, out int index)) id = ParseAddress(Query.AddressFromIndex(index)).id;
        if (!_registry._tracks.ContainsKey(id)) return null;

        return _registry._tracks[id];
    }
    public static Playlist Playlist(string id) {
        if (int.TryParse(id, out int index)) id = ParseAddress(Query.AddressFromIndex(index)).id;
        if (!_registry._playlists.ContainsKey(id)) return null;

        return _registry._playlists[id];
    }
    public static Artist Artist(string id) {
        if (int.TryParse(id, out int index)) id = ParseAddress(Query.AddressFromIndex(index)).id;
        if (!_artists.ContainsKey(id)) return null;
    
        return _artists[id];
    }
    public static Album Album(string id) {
        if (int.TryParse(id, out int index)) id = ParseAddress(Query.AddressFromIndex(index)).id;
        if (!_albums.ContainsKey(id)) return null;

        return _albums[id];
    }
    public static string[] Tracks() {
        return [.. _registry._tracks.Keys];
    }
    public static string[] Playlists() {
        return [.. _registry._playlists.Keys];
    }
    public static string[] Artists() {
        return [.. _artists.Keys];
    }
    public static string[] Albums() {
        return [.. _albums.Keys];
    }
    public static string[] Search(string query, int results) {
        (AddressType addressType, string _) = ParseAddress(query, true);
       
        Cosine stringComparer = new Cosine();
        
        string[] topResults;
        if (addressType == AddressType.INVALID) {
            topResults = _addressLookup
                .OrderByDescending(pair => stringComparer.Similarity(pair.Value, query))
                .ThenBy(pair => pair.Key)
                .Select(pair => pair.Key)
                .Take(results)
                .ToArray();
        }
        else {
            topResults = _addressLookup
                .Where(pair => pair.Key.StartsWith(addressType.ToString(), StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(pair => stringComparer.Similarity(pair.Value, query))
                .ThenBy(pair => pair.Key)
                .Select(pair => pair.Key)
                .Take(results)
                .ToArray();
        }
                
        return topResults;
    }
    
    public static void AddTrack(Track track) {
        _registry._tracks[TrackId(track)] = track;
        _updateSavedData();
        _updateNonPersistantData();
    }
    public static void Delete(string address) {
        static void deleteTrack(string id) {
            _registry._tracks.Remove(id);
            FileHandler.Delete(id);
        }

        string[] deletionQueue = TracksFromAddress(address);

        for (int i = 0; i < deletionQueue.Length; ++i) {
            string trackId = deletionQueue[i];
            for(int j = 0; j < _registry._playlists.Count; ++j) {
                _registry._playlists.ElementAt(j).Value.Tracks.Remove(trackId);
            }
            deleteTrack(trackId);
        }

        _updateSavedData();
        _updateNonPersistantData();
    }

    public static void AddToPlaylist(string id, string address) {
        if (int.TryParse(id, out int index)) id = ParseAddress(Query.AddressFromIndex(index)).id;
        if (Playlist(id) == null) {
            _registry._playlists.Add(id, new([]));
        }

        string[] addQueue = TracksFromAddress(address);
        for (int i = 0; i < addQueue.Length; ++i) {
            _registry._playlists[id].Tracks.Add(addQueue[i]);
        }

        _updateSavedData();
    }
    public static void RemoveFromPlaylist(string id, string address) {
        if (int.TryParse(id, out int index)) id = ParseAddress(Query.AddressFromIndex(index)).id;
        if (Playlist(id) == null) return;

        string[] removeQueue = TracksFromAddress(address);
        for (int i = 0; i < removeQueue.Length; ++i) {
            _registry._playlists[id].Tracks.Remove(removeQueue[i]);
        }

        _updateSavedData();
    }
    public static void DeletePlaylist(string id) {
        if (int.TryParse(id, out int index)) id = ParseAddress(Query.AddressFromIndex(index)).id;
        _registry._playlists.Remove(id);
        _updateSavedData();
    }

    public static string[] TracksFromAddress(string address) {
        if (int.TryParse(address, out int index)) address = Query.AddressFromIndex(index);

        if (!ValidAddress(address)) return [];

        (AddressType type, string id) = ParseAddress(address);

        return type switch
        {
            AddressType.PLAYLIST => [.. Playlist(id).Tracks],
            AddressType.ARTIST => [.. Artist(id).Tracks],
            AddressType.ALBUM => [.. Album(id).Tracks],
            _ => [id],
        };
    }
    public static bool ValidAddress(string address) {
        if (address == default) return false;

        (AddressType type, string id) = ParseAddress(address);

        return type switch
        {
            AddressType.TRACK => _registry._tracks.ContainsKey(id),
            AddressType.PLAYLIST => _registry._playlists.ContainsKey(id),
            AddressType.ARTIST => _artists.ContainsKey(id),
            AddressType.ALBUM => _albums.ContainsKey(id),
            _ => false,
        };
    }
    public static (AddressType type, string id) ParseAddress(string address, bool disableFuzzySearch = false) {
        if (!disableFuzzySearch) address = Search(address, 1)[0];

        string[] parts = address.Split(':', 2);

        if (parts.Length < 2) {
            return (AddressType.INVALID, default);
        }
        
        string type = parts[0].ToUpper();
        string id = parts[1];

        if (!Enum.TryParse(type, out AddressType typeEnum)) {
            return (AddressType.INVALID, default);
        }

        return (typeEnum, id);
    }
    public static string TrackId(Track track) {
        return $"{track.Title}@{track.Album}";
    }

    public static void Initialise() {
        _registry = FileHandler.LoadRegistry();
        _updateNonPersistantData();
    }

    private static void _updateSavedData() {
        _orderAddresses();

        FileHandler.SaveRegistry(_registry);
    }
    private static void _updateNonPersistantData() {
        _artists = new();
        _albums = new();

        for (int i = 0; i < _registry._tracks.Count; ++i) {
            Track track = _registry._tracks.ElementAt(i).Value;
            _constructArtistsFromTrack(track);
            _constructAlbumsFromTrack(track);
        }

        _orderAddresses();

        for (int i = 0; i < _albums.Count; ++i) {
            string albumId = _albums.ElementAt(i).Key;

            List<string> tracks = [.. Album(albumId).Tracks.OrderBy(trackId => Track(trackId).TrackNumber)];

            Album album = new(tracks, Album(albumId).Artists);
            _albums[albumId] = album;
        }
    }
    private static void _orderAddresses() {
        Dictionary<string, Track> tracks = _registry._tracks.OrderBy(kv => kv.Key).ToDictionary(kv => kv.Key, kv => kv.Value);;
        Dictionary<string, Playlist> playlists = _registry._playlists.OrderBy(kv => kv.Key).ToDictionary(kv => kv.Key, kv => kv.Value);;
        _registry = new Registry(tracks, playlists);

        _artists = _artists.OrderBy(kv => kv.Key).ToDictionary(kv => kv.Key, kv => kv.Value);
        _albums = _albums.OrderBy(kv => kv.Key).ToDictionary(kv => kv.Key, kv => kv.Value);
    }
    private static void _constructArtistsFromTrack(Track track) {
        for (int i = 0; i < track.Artists.Length; ++i) {
            if (_artists.ContainsKey(track.Artists[i])) {
                _artists[track.Artists[i]].Tracks.Add(TrackId(track));
                _artists[track.Artists[i]].Albums.Add(track.Album);
            }
            else {
                List<string> tracks = [TrackId(track)];
                List<string> albums = [track.Album];

                Artist artist = new(tracks, albums);
                _artists.Add(track.Artists[i], artist);
            }
        }
    }
    private static void _constructAlbumsFromTrack(Track track) {
        if (_albums.ContainsKey(track.Album)) {
            _albums[track.Album].Tracks.Add(TrackId(track));
            for(int i = 0; i < track.Artists.Length; ++i) {
                _albums[track.Album].Artists.Add(track.Artists[i]);
            }
        }
        else {
            List<string> tracks = [TrackId(track)];
            List<string> artists = [.. track.Artists];

            Album album = new(tracks, artists);
            _albums.Add(track.Album, album);
        }

        _generateAddressLookup();
    }
    private static void _generateAddressLookup() {
        _addressLookup = _registry._tracks
            .ToDictionary(track => $"track:{track.Key}", track => track.Value.Title.ToLower())
            .Concat(
                _registry._playlists.ToDictionary(playlist => $"playlist:{playlist.Key}", playlist => playlist.Key.ToLower())
            )
            .Concat(
                _artists.ToDictionary(artist => $"artist:{artist.Key}", artist => artist.Key.ToLower())
            )
            .Concat(
                _albums.ToDictionary(album => $"album:{album.Key}", album => album.Key.ToLower())
            )
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }
}

public enum AddressType {
    INVALID,
    TRACK,
    PLAYLIST,
    ARTIST,
    ALBUM
}

public record Registry(
 	Dictionary<string, Track> _tracks,
    Dictionary<string, Playlist> _playlists
);

public record Track(
    string Title,
    string[] Artists,
    string Album,
    uint TrackNumber,
    double Duration
);
public record Playlist(
    List<string> Tracks
);

public record Artist(
	List<string> Tracks,
	List<string> Albums
);
public record Album(
	List<string> Tracks,
	List<string> Artists
);