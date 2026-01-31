using System.Diagnostics;
using ilikefrogs101.Shutdown;
using MiniAudioEx.Core.StandardAPI;

namespace ilikefrogs101.MusicPlayer;
public class MiniAudioExBackend : AudioBackend {
    private const int SAMPLE_RATE = 44100;
    private const int CHANNELS = 2;

    private bool _paused = false;
    private ulong _pauseFrame = 0;

    private AudioSource _source;
    private AudioClip _clip;

    private bool _subscribedToCallback;
    private bool _isRunning;
    private Thread _updateThread;

    protected override void _playInternal() {
        _clip = new AudioClip(Path.Combine(FileHandler.TrackStoragePath(), CurrentTrack));
        _source.Play(_clip);

        if(!_subscribedToCallback) {
            _source.End += _finished;
            _subscribedToCallback = true;
        }
    }
    public override void Pause() {
        _pauseFrame = _source.Cursor;
        _source.Stop();
        _paused = true;
    }
    public override void Resume() {
        _source.Play(_clip);
        _source.Cursor = _pauseFrame;
        _paused = false;
    }
    public override void Restart() {
        _source.Cursor = 0;
    }
    public override void Stop() {
        _source.Stop();
    }
    public override void Forward(ulong seconds) {
        ulong cursor = _source.Cursor + (seconds * SAMPLE_RATE);
        if(cursor < _source.Cursor) {
            _source.Cursor = _source.Length;
            return;
        }
        _source.Cursor = cursor;
    }
    public override void Backward(ulong seconds) {
        ulong cursor = _source.Cursor - (seconds * SAMPLE_RATE);
        if(cursor > _source.Cursor) {
            _source.Cursor = 0;
            return;
        }
        _source.Cursor = cursor;
    }
    public override void SetVolume(float volume) {
        _source.Volume = volume;
    }
    public override ulong Progress() {
        if(_paused) return _pauseFrame / SAMPLE_RATE;

        return _source.Cursor / SAMPLE_RATE;
    }
    public override ulong Length() {
        return _source.Length / SAMPLE_RATE;
    }
    public MiniAudioExBackend() {
        AudioContext.Initialize(SAMPLE_RATE, CHANNELS);
        _source = new AudioSource();
        _isRunning = true;

        _updateThread = new Thread(UpdateLoop)
        {
            IsBackground = true
        };
        _updateThread.Start();

        ShutdownHandler.OnShutdownCalled += Dispose;
    }

    private void UpdateLoop()
    {
        while (_isRunning)
        {
            AudioContext.Update();
            Thread.Sleep(10);
        }
    }
    public void Dispose()
    {
        if (!_isRunning) return;

        _isRunning = false;
        _updateThread.Join();
        AudioContext.Deinitialize();
    }
}