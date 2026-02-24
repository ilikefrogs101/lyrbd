using ilikefrogs101.Logging;
using ilikefrogs101.Shutdown;
using MiniAudioEx.Core.StandardAPI;

namespace Lyrbd.Daemon;
public class MiniAudioExBackend : AudioBackend {
    private const int SAMPLE_RATE = 48000;
    private const int CHANNELS = 2;

    private Thread _audioThread;

    private AudioApp _app;
    private AudioSource _source;
    private AudioClip _clip;

    private bool _paused;
    private ulong _pausedFrame;

    public override void Play(string path) { 
        if (!File.Exists(path)) {
            Log.ErrorMessage($"Cannot load {path}, file not found");
            return;
        }

        _clip = new AudioClip(path);
        _source.Play(_clip);
    }
    public override void Pause(bool paused) {
        _paused = paused;
        if (_paused) {
            _pausedFrame = _source.Cursor;
            _source.Stop();
        }
        else {
            _source.Play(_clip);
            _source.Cursor = _pausedFrame;
        }
    }
    public override void Restart() {
        _source.Cursor = 0;
    }
    public override void Stop() {
        _source.Stop();
    }
    public override void Forward(ulong seconds) {
        ulong frame = _source.Cursor + _secondsToFrames(seconds);

        if (frame < _source.Cursor || frame == _source.Length) {
            frame = _source.Length;
        }
        _source.Cursor = frame;     
    }
    public override void Backward(ulong seconds) {
        ulong frame = _source.Cursor - _secondsToFrames(seconds);

        if (frame > _source.Cursor) {
            frame = 0;
        }
        _source.Cursor = frame; 
    }
    public override void SkipTo(ulong seconds) {
        ulong frame = _secondsToFrames(seconds);
        frame = Math.Clamp(frame, 0, _source.Length);

        _source.Cursor = frame;
    }
    public override void SetVolume(float volume) {
        _source.Volume = volume;
    }

    public override bool IsPaused() {
        return _paused;
    }
    public override float Volume() {
        return _source.Volume;
    }
    public override ulong TrackProgress() {
        if (_paused) return _framesToSeconds(_pausedFrame);

        return _framesToSeconds(_source.Cursor);
    }

    private static ulong _secondsToFrames(ulong seconds) {
        return seconds * SAMPLE_RATE;
    }
    private static ulong _framesToSeconds(ulong frames) {
        return frames / SAMPLE_RATE;
    }

    public MiniAudioExBackend() {
        _app = new AudioApp(SAMPLE_RATE, CHANNELS);
        _app.Loaded += () => _source = new AudioSource();
        _app.Loaded += () => _source.End += _trackFinished;

        _audioThread = new Thread(() => _app.Run()) {
            IsBackground = true
        };

        _audioThread.Start();

        ShutdownHandler.OnShutdownCalled += AudioContext.Deinitialize;
    }
}