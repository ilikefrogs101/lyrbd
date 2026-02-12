using System.Reflection;
using System.Text.Json;

namespace Lyrbd.Daemon;
public static class FileHandler {
    public static bool Exists(string id) {
        return File.Exists(DataPath(id));
    }
    public static void Import(string path, string id) {
        File.Copy(path, DataPath(id), true);
    }
    public static void Export(string path, string id) {
        File.Copy(path, DataPath(id), true);
    }
    public static void Delete(string id) {
        File.Delete(DataPath(id));
    }

    public static void SaveRegistry(Registry data) {
        string json = JsonSerializer.Serialize(data);
        File.WriteAllText(DataPath("registry.json"), json);
    }
    public static Registry LoadRegistry() {        
        if (!Exists("registry.json")) return new(new(), new());

        string json = File.ReadAllText(DataPath("registry.json"));
        return JsonSerializer.Deserialize<Registry>(json);
    }
    public static string DataPath(string id) {
        string directory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), 
            ".local", 
            "share", 
            Assembly.GetExecutingAssembly().GetName().Name
        );

        if (!Directory.Exists(directory)) {
            Directory.CreateDirectory(directory);
        }

        return Path.Combine(directory, id);
    }
}