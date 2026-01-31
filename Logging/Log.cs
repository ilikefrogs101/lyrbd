namespace ilikefrogs101.Logging;
public static class Log {
    public static event Action<string> OnResponse;
    public static event Action<string> OnDebugInformation;
    public static event Action<string> OnErrorInformation;
    public static void OutputResponse(string message) {
        OnResponse?.Invoke(message);
    }
    public static void DebugMessage(string message) {
        OnDebugInformation?.Invoke(message);
    }
    public static void ErrorMessage(string message) {
        OnErrorInformation?.Invoke(message);
    }
}