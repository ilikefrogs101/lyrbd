using System.Reflection;
using System.Text.Json;

namespace ilikefrogs101.MusicPlayer;
public static class FileHandler {
    public static string TitleOverride = null;
    public static string[] ArtistsOverride = null;
    public static string AlbumOverride = null;
    public static uint TrackNumberOverride = uint.MaxValue;

    public static void UpdatePersistantData(PersistantData persistantData) {
        string json = JsonSerializer.Serialize(persistantData);
        File.WriteAllText(_persistantDataPath(), json);
    }
    public static PersistantData LoadPersistantData() {
        string json = File.ReadAllText(_persistantDataPath());
        return JsonSerializer.Deserialize<PersistantData>(json);
    }

    public static void ImportTrack(string path) {
        string title = _getTitleFromFile(path);
        string[] artists = _getArtistsFromFile(path);
        string album = _getAlbumFromFile(path);
        uint trackNumber = _getTrackNumberFromFile(path);
        _resetOverrides();

        Track track = new Track(title, artists, album, trackNumber);
        _moveTrack(track, path);
        TrackManager.AddTrack(track);
    }
    private static void _resetOverrides() {
        TitleOverride = null;
        ArtistsOverride = null;
        AlbumOverride = null;
        TrackNumberOverride = uint.MaxValue;
    }
    private static string _getTitleFromFile(string path) {
        if(TitleOverride != null) {
            return TitleOverride;
        }

        TagLib.File tagFile = TagLib.File.Create(path);

        if(!string.IsNullOrWhiteSpace(tagFile.Tag.Title)) {
            return tagFile.Tag.Title;
        }
        
        return Path.GetFileNameWithoutExtension(path);
    }
    private static string[] _getArtistsFromFile(string path) {
        if(ArtistsOverride != null) {
            return ArtistsOverride;
        }

        TagLib.File tagFile = TagLib.File.Create(path);

        return tagFile.Tag.Performers;
    }
    private static string _getAlbumFromFile(string path) {
        if(AlbumOverride != null) {
            return AlbumOverride;
        }

        TagLib.File tagFile = TagLib.File.Create(path);

        if(!string.IsNullOrWhiteSpace(tagFile.Tag.Album)) {
            return tagFile.Tag.Album;
        }

        return Path.GetFileName(Path.GetDirectoryName(path));
    }
    private static uint _getTrackNumberFromFile(string path) {
        if(TrackNumberOverride != uint.MaxValue) {
            return TrackNumberOverride;
        }

        TagLib.File tagFile = TagLib.File.Create(path);

        return tagFile.Tag.Track;
    }

    private static void _moveTrack(Track track, string path) {
        string storagePath = Path.Combine(TrackStoragePath(), TrackManager.GetTrackStorageID(track));
        File.Copy(path, storagePath);

        TagLib.File tagFile = TagLib.File.Create(storagePath);
        tagFile.Tag.Clear();
        tagFile.Save();
    }

    private static string _persistantDataPath() {
        string path = Path.Combine(
            TrackStoragePath(),
            "persistant.json"
        );

        return path;
    }
    public static string TrackStoragePath() {
        string path = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), 
            ".local", 
            "share", 
            Assembly.GetExecutingAssembly().GetName().Name
        );

        if(!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
        }

        return path;
    }
}