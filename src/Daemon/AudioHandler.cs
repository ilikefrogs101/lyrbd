namespace Lyrbd.Daemon;
public static class AudioHandler {
    public static event Action OnCurrentTrackEnd;
    private static AudioBackend _backend;

    public static void Play(string id) {
        if (_backend == null) return;

        _backend.Play(FileHandler.DataPath(id));
    }
    public static void Pause(bool paused) {
        if (_backend == null) return;

        _backend.Pause(paused);
    }
    public static void Restart() {
        if (_backend == null) return;

        _backend.Restart();
    }
    public static void Stop() {
        if (_backend == null) return;

        _backend.Stop();
    }
    public static void Forward(ulong seconds) {
        if (_backend == null) return;

        _backend.Forward(seconds);
    }
    public static void Backward(ulong seconds) {
        if (_backend == null) return;

        _backend.Backward(seconds);
    }
    public static void SkipTo(ulong seconds) {
        if (_backend == null) return;

        _backend.SkipTo(seconds);
    }
    public static void SetVolume(float volume) {
        if (_backend == null) return;
        
        _backend.SetVolume(volume);
    }

    public static bool IsPaused() {
        if (_backend == null) return true;

        return _backend.IsPaused();
    }
    public static float Volume() {
        if (_backend == null) return 1;

        return _backend.Volume();
    }
    public static ulong TrackProgress() {
        if (_backend == null) return 0;

        return _backend.TrackProgress();
    }

    public static void Initialise() {
        if (_backend != null) return;
        
        _backend = new MiniAudioExBackend();
        _backend.OnCurrentTrackEnd += () => OnCurrentTrackEnd?.Invoke();
    }
}