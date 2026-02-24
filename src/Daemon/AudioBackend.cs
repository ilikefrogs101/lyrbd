using System;

namespace Lyrbd.Daemon;
public abstract class AudioBackend {
    public event Action OnCurrentTrackEnd;

    public abstract void Play(string path);
    public abstract void Pause(bool paused);
    public abstract void Restart();
    public abstract void Stop();
    public abstract void Forward(double seconds);
    public abstract void Backward(double seconds);
    public abstract void SkipTo(double seconds);
    public abstract void SetVolume(float volume);

    public abstract bool IsPaused();
    public abstract float Volume();
    public abstract double TrackProgress();

    protected void _trackFinished() {
        OnCurrentTrackEnd?.Invoke();
    }
}