namespace ilikefrogs101.Logging;
public static class Log {
    public static Action<string> Response;
    public static Action<string> DebugInformation;
    public static Action<string> ErrorInformation;
    public static void OutputResponse(string message) {
        Response?.Invoke(message);
    }
    public static void DebugMessage(string message) {
        DebugInformation?.Invoke(message);
    }
    public static void ErrorMessage(string message) {
        ErrorInformation?.Invoke(message);
    }
}