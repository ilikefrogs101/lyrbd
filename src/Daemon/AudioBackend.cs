using System;

namespace Lyrbd.Daemon;
public abstract class AudioBackend {
    public event Action OnCurrentTrackEnd;

    public abstract void Play(string path);
    public abstract void Pause(bool paused);
    public abstract void Restart();
    public abstract void Stop();
    public abstract void Forward(ulong seconds);
    public abstract void Backward(ulong seconds);
    public abstract void SetVolume(float volume);

    public abstract bool IsPaused();
    public abstract float Volume();
    public abstract ulong TrackProgress();
    public abstract ulong TrackLength();

    protected void _trackFinished() {
        OnCurrentTrackEnd?.Invoke();
    }
}