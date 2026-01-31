using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ilikefrogs101.MusicPlayer;
public static class FileHandler {
    public static string TitleOverride = null;
    public static string[] ArtistsOverride = null;
    public static string AlbumOverride = null;
    public static uint TrackNumberOverride = uint.MaxValue;
    public static bool SpaceBeforeCapitals = false;
    public static bool UnderscoreMeansSpace = false;
    public static bool HyphenMeansSpace = false;
    public static bool AutoCapitalise = false;
    public static bool StripNonLetters = false;

    public static void UpdatePersistantData(PersistantData persistantData) {
        string json = JsonSerializer.Serialize(persistantData);
        File.WriteAllText(_persistantDataPath(), json);
    }
    public static PersistantData LoadPersistantData() {
        if(File.Exists(_persistantDataPath())) {
            string json = File.ReadAllText(_persistantDataPath());
            return JsonSerializer.Deserialize<PersistantData>(json);
        }

        return new(new(), new());
    }

    public static void Import(string path) {
        FileAttributes pathAttributes = File.GetAttributes(path);

        if(pathAttributes.HasFlag(FileAttributes.Directory)) {
            _bulkImport(path);
            _resetOverrides();
        }
        else {
            _importTrack(path);
            _resetOverrides();
        }
    }
    private static void _bulkImport(string path) {
        string[] paths = Directory.GetFiles(path);

        for(int i = 0; i < paths.Length; ++i) {
            _importTrack(paths[i]);
        }
    }
    public static void _importTrack(string path) {
        string title = _getTitleFromFile(path);
        string[] artists = _getArtistsFromFile(path);
        string album = _getAlbumFromFile(path);
        uint trackNumber = _getTrackNumberFromFile(path);

        Track track = new Track(title, artists, album, trackNumber);
        _moveTrack(TrackManager.GetTrackID(track), path);
        TrackManager.AddTrack(track);
        TrackManager.UpdatePersistantData();
    }
    private static void _resetOverrides() {
        TitleOverride = null;
        ArtistsOverride = null;
        AlbumOverride = null;
        TrackNumberOverride = uint.MaxValue;
        SpaceBeforeCapitals = false;
        UnderscoreMeansSpace = false;
        HyphenMeansSpace = false;
        AutoCapitalise = false;
        StripNonLetters = false;
    }
    private static string _getTitleFromFile(string path) {
        if(TitleOverride != null) {
            return TitleOverride;
        }

        TagLib.File tagFile = TagLib.File.Create(path);

        if(!string.IsNullOrWhiteSpace(tagFile.Tag.Title)) {
            return tagFile.Tag.Title;
        }

        string title = Path.GetFileNameWithoutExtension(path);
        
        if(UnderscoreMeansSpace) {
            title = title.Replace('_', ' ');
        }
        if(HyphenMeansSpace) {
            title = title.Replace('-', ' ');
        }
        if(SpaceBeforeCapitals) {
            title = Regex.Replace(title, "(?<!^)(?<!\\s)([A-Z])", " $1");
        }
        if(AutoCapitalise) {
            title = Regex.Replace(title, @"(?<=^|\s)\p{L}", m => m.Value.ToUpper());
        }
        if(StripNonLetters) {
            title = Regex.Replace(title, @"[^\p{L}]", "");
        }

        return title;
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

    private static void _moveTrack(string id, string path) {
        string storagePath = Path.Combine(TrackStoragePath(), TrackManager.GetTrackStorageID(id));
        File.Copy(path, storagePath, true);

        //TagLib.File tagFile = TagLib.File.Create(storagePath);
        //tagFile.Tag.Clear();
        //tagFile.Save();
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