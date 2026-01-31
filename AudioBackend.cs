namespace ilikefrogs101.MusicPlayer;
public abstract class AudioBackend {
    public string CurrentTrack { get; private set; }
    public event Action OnCurrentTrackEnd;

    protected void _finished() {
        OnCurrentTrackEnd?.Invoke();
    }

    protected abstract void _playInternal();
    
    public abstract void Pause();
    public abstract void Resume();
    public abstract void Restart();
    public abstract void Stop();
    public abstract void Forward(ulong seconds);
    public abstract void Backward(ulong seconds);
    public abstract void SetVolume(float volume);
    public abstract ulong Progress();
    public abstract ulong Length();
    public abstract float Volume();
    
    public void Play(string id) {
        CurrentTrack = id;
        _playInternal();
    }
}