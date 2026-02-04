using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using ilikefrogs101.Logging;

namespace Lyrbd.Daemon;
public static class FileHandler {
    public static string TitleOverride = default;
    public static string[] ArtistsOverride = default;
    public static string AlbumOverride = default;
    public static uint TrackNumberOverride = default;
    public static bool AutoSpace = false;
    public static bool AutoCapitalise = false;
    public static bool StripNumbers = false;
    public static bool StripPunctuation = false;

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
        if(!File.Exists(path)) {
            Log.ErrorMessage($"Cannot import \"{path}\", that file does not exists");
            return;            
        }

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
        TitleOverride = default;
        ArtistsOverride = default;
        AlbumOverride = default;
        TrackNumberOverride = default;
        AutoSpace = false;
        AutoCapitalise = false;
        StripNumbers = false;
        StripPunctuation = false;
    }
    private static string _getTitleFromFile(string path) {
        if(TitleOverride != default) {
            return TitleOverride;
        }

        TagLib.File tagFile = TagLib.File.Create(path);

        if(!string.IsNullOrWhiteSpace(tagFile.Tag.Title)) {
            return tagFile.Tag.Title;
        }

        string title = Path.GetFileNameWithoutExtension(path);
        
        if(StripPunctuation) {
            title = Regex.Replace(title, @"[^\w\s]", "");
        }
        if(StripNumbers) {
            title = Regex.Replace(title, @"\d+", "");
        }
        if(AutoSpace) {
            title = title.Replace('_', ' ');
            title = title.Replace('-', ' ');
            title = Regex.Replace(title, "(?<!^)([A-Z])", " $1");
        }
        title = Regex.Replace(title, @"\s+", " ").Trim();
        if(AutoCapitalise) {
            title = Regex.Replace(title, @"(^|\s)(\S)", match =>
            {
                return match.Groups[1].Value + match.Groups[2].Value.ToUpper();
            });
        }
        return title;
    }
    private static string[] _getArtistsFromFile(string path) {
        if(ArtistsOverride != default) {
            return ArtistsOverride;
        }

        TagLib.File tagFile = TagLib.File.Create(path);

        return tagFile.Tag.Performers;
    }
    private static string _getAlbumFromFile(string path) {
        if(AlbumOverride != default) {
            return AlbumOverride;
        }

        TagLib.File tagFile = TagLib.File.Create(path);

        if(!string.IsNullOrWhiteSpace(tagFile.Tag.Album)) {
            return tagFile.Tag.Album;
        }

        return Path.GetFileName(Path.GetDirectoryName(path));
    }
    private static uint _getTrackNumberFromFile(string path) {
        if(TrackNumberOverride != default) {
            return TrackNumberOverride;
        }

        TagLib.File tagFile = TagLib.File.Create(path);

        return tagFile.Tag.Track;
    }

    public static void DeleteTrack(string id) {
        string storagePath = Path.Combine(TrackStoragePath(), TrackManager.GetTrackStorageID(id));
        File.Delete(storagePath);
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