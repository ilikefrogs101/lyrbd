using ilikefrogs101.Logging;

namespace ilikefrogs101.Shutdown;
public static class ShutdownHandler {
    public static event Action OnShutdownCalled;

    public static void RequestShutdown() {
        Log.DebugMessage("Shutting down...");
        OnShutdownCalled.Invoke();
        Environment.Exit(0);
    }
}