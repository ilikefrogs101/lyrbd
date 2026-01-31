namespace ilikefrogs101.MusicPlayer;
public abstract class AudioBackend {
    public Track CurrentTrack { get; private set; }
    public event Action FinishedCurrentTrack;

    protected void _finished() {
        FinishedCurrentTrack?.Invoke();
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
    
    public void Play(Track track) {
        CurrentTrack = track;
        _playInternal();
    }
}