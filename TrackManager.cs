using System.Security.Cryptography;
using System.Text;

namespace ilikefrogs101.MusicPlayer;
public static class TrackManager {
    private static PersistantData _persistantData;
    private static Dictionary<string, Artist> _artists;
    private static Dictionary<string, Album> _albums;

    public static void Initialise() {
        _persistantData = FileHandler.LoadPersistantData();
        GenerateNonPersistantData();
    }

    public static Track GetTrack(string id) {
        return _persistantData._tracks[id];
    }
    public static Playlist GetPlaylist(string id) {
        return _persistantData._playlists[id];
    }
    public static Artist GetArtist(string id) {
        return _artists[id];
    }
    public static Album GetAlbum(string id) {
        return _albums[id];
    }
    public static string[] GetTrackList() {
        return [.. _persistantData._tracks.Keys];
    }
    public static string[] GetPlaylistList() {
        return [.. _persistantData._playlists.Keys];
    }
    public static string[] GetArtistList() {
        return [.. _artists.Keys];
    }
    public static string[] GetAlbumList() {
        return [.. _albums.Keys];
    }

    public static void AddTrack(Track track) {
        _persistantData._tracks[GetTrackID(track)] = track;
        GenerateNonPersistantData();
    }

    public static void UpdatePersistantData() {
        FileHandler.UpdatePersistantData(_persistantData);
    }
    public static void GenerateNonPersistantData() {
        _artists = new();
        _albums = new();

        for(int i = 0; i < _persistantData._tracks.Count; ++i) {
            Track track = _persistantData._tracks.ElementAt(i).Value;
            _makeArtistsFromTrack(track);
            _makeAlbumFromTrack(track);
        }
    }

    private static void _makeArtistsFromTrack(Track track) {
        for(int i = 0; i < track.Artists.Length; ++i) {
            if(_artists.ContainsKey(track.Artists[i])) {
                _artists[track.Artists[i]].Tracks.Add(GetTrackID(track));
                _artists[track.Artists[i]].Albums.Add(track.Album);
            }
            else {
                HashSet<string> tracks = [GetTrackID(track)];
                HashSet<string> albums = [track.Album];

                Artist artist = new(track.Artists[i], tracks, albums);
                _artists.Add(track.Artists[i], artist);
            }
        }
    }
    private static void _makeAlbumFromTrack(Track track) {
        if(_albums.ContainsKey(track.Album)) {
            _albums[track.Album].Tracks.Add(GetTrackID(track));
            for(int i = 0; i < track.Artists.Length; ++i) {
                _albums[track.Album].Artists.Add(track.Artists[i]);
            }
        }
        else {
            HashSet<string> tracks = [GetTrackID(track)];
            HashSet<string> artists = new(track.Artists);

            Album album = new(track.Album, tracks, artists);
            _albums.Add(track.Album, album);
        }
    }

    public static string GetTrackID(Track track) {
        return $"{track.Album}/{track.Title}";
    }
    public static string GetTrackStorageID(string id) {
        StringBuilder output = new StringBuilder();

        using (SHA256 sha256Hash = SHA256.Create()) {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(id));
            foreach (byte b in bytes) {
                output.Append(b.ToString("x2"));
            }
        }
            
        return output.ToString();
    }
}

public record PersistantData(
 	Dictionary<string, Track> _tracks,
    Dictionary<string, Playlist> _playlists
);

public record Track(
    string Title,
    string[] Artists,
    string Album,
    uint TrackNumber
);
public record Playlist(
    string Title,
    string[] Tracks
);

public record Artist(
	string Name,
	HashSet<string> Tracks,
	HashSet<string> Albums
);
public record Album(
	string Title,
	HashSet<string> Tracks,
	HashSet<string> Artists
);