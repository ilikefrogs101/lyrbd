using System.Linq;
using System.Text.RegularExpressions;
using ilikefrogs101.CustomExtensions;
using ilikefrogs101.FileHandling;

namespace Lyrbd.Daemon;
public static class TrackImporter {
    public static string TitleOverride = default;
    public static string[] ArtistsOverride = default;
    public static string AlbumOverride = default;
    public static uint TrackNumberOverride = default;
    public static bool AutoSpace = false;
    public static bool AutoCapitalise = false;
    public static bool StripNumbers = false;
    public static bool StripPunctuation = false;
    public static bool Single = false;
    public static bool AlphabeticalTrackNumbers = false;

    public static void Import(string path) {
        _import(path);
        _resetOverrides();
    }
    
    private static void _import(string path) {
        FileAttributes pathAttributes = File.GetAttributes(path);

        if(pathAttributes.HasFlag(FileAttributes.Directory)) {
            _importFolder(path);
        }
        else {
            _importFile(path);
        }
    }
    private static void _importFile(string path) {
        string title = _getTitleFromFile(path);
        string[] artists = _getArtistsFromFile(path);
        string album = _getAlbumFromFile(path);
        uint trackNumber = _getTrackNumberFromFile(path);
        double duration = _getDurationFromFile(path);

        Track track = new Track(title, artists, album, trackNumber, duration);
        string trackId = LibraryManager.TrackId(track);

        FileSystem.Copy(new(path), new(trackId, PathOrigin.DATA));
        LibraryManager.AddTrack(track);
    }
    private static void _importFolder(string path) {
        TrackNumberOverride = default;
        
        string[] paths = [.. Directory.GetFiles(path).OrderBy(x => PathUtils.ResolvePathName(new(x)))];        
        for (uint i = 0; i < paths.Length; ++i) {
            _import(paths[i]);

            if (AlphabeticalTrackNumbers) {
                TrackNumberOverride = i+1;
            }
        }
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
        Single = false;
        AlphabeticalTrackNumbers = false;
    }
    private static string _getTitleFromFile(string path) {
        if(TitleOverride != default) {
            return TitleOverride.Standardise();
        }

        TagLib.File tagFile = TagLib.File.Create(path);

        if(!string.IsNullOrWhiteSpace(tagFile.Tag.Title)) {
            return tagFile.Tag.Title.Standardise();
        }

        string title = PathUtils.ResolvePathName(new(path)).Standardise();

        if(AutoSpace) {
            title = title.Replace('_', ' ');
            title = title.Replace('-', ' ');
            title = Regex.Replace(title, "(?<!^)([A-Z])", " $1");
        }
        
        if(StripPunctuation) title = title.RemovePunctuation();
        if(StripNumbers) title = title.RemoveNumbers();

        if(AutoCapitalise) {
            title = char.ToUpperInvariant(title[0]) + title[1..];
            title = Regex.Replace(title, @"(^|\s)(\S)", match => {
                return match.Groups[1].Value + match.Groups[2].Value.ToUpper();
            });
        }

        return title.Standardise();
    }
    private static string[] _getArtistsFromFile(string path) {
        string[] artists;

        if(ArtistsOverride != default) {
            artists = new string[ArtistsOverride.Length];
            for (int i = 0; i < artists.Length; ++i) {
                artists[i] = ArtistsOverride[i].Standardise();
            }           
            return artists;
        }

        TagLib.File tagFile = TagLib.File.Create(path);
        artists = new string[tagFile.Tag.Performers.Length];
        for (int i = 0; i < artists.Length; ++i) {
            artists[i] = tagFile.Tag.Performers[i].Standardise();
        }
        return artists;
    }
    private static string _getAlbumFromFile(string path) {
        if(AlbumOverride != default) {
            return AlbumOverride.Standardise();
        }
        if(Single) {
            return _getTitleFromFile(path);
        }

        TagLib.File tagFile = TagLib.File.Create(path);

        if(!string.IsNullOrWhiteSpace(tagFile.Tag.Album)) {
            return tagFile.Tag.Album.Standardise();
        }

        return PathUtils.ResolvePathName(PathUtils.ResolveDirectory(new(path))).Standardise();
    }
    private static uint _getTrackNumberFromFile(string path) {
        if (TrackNumberOverride != default) {
            return TrackNumberOverride;
        }

        using (TagLib.File tagFile = TagLib.File.Create(path)) {
            if (tagFile.Tag.Track != 0) {
                return tagFile.Tag.Track;
            }
        }

        string fileName = PathUtils.ResolvePathName(new(path));
        Match match = Regex.Match(fileName, @"^\s*(\d{1,3})");
        if (match.Success && uint.TryParse(match.Groups[1].Value, out uint trackNumber)) {
            return trackNumber;
        }

        return 0;
    }
    private static double _getDurationFromFile(string path) {
        using TagLib.File tagFile = TagLib.File.Create(path);
        return tagFile.Properties.Duration.TotalSeconds;
    }
}
