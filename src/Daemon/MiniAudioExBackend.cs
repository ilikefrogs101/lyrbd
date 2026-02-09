using System.IO;
using System.Threading;
using ilikefrogs101.Shutdown;
using ilikefrogs101.Logging;
using MiniAudioEx.Core.StandardAPI;

namespace Lyrbd.Daemon;
public class MiniAudioExBackend : AudioBackend {
    private const int SAMPLE_RATE = 48000;
    private const int CHANNELS = 2;

    private bool _paused = false;
    private ulong _pauseFrame = 0;

    private AudioApp _app;
    private AudioSource _source;
    private AudioClip _clip;

    private bool _subscribedToCallback;
    private bool _initialised;
    private Thread _audioThread;

    protected override void _playInternal() {
        string path = Path.Combine(FileHandler.TrackStoragePath(), TrackManager.GetTrackStorageID(CurrentTrack));

        if(!File.Exists(path)) {
            Log.ErrorMessage($"Cannot load {path}, file not found");
            return;
        }

        _clip = new AudioClip(path);
        _source.Play(_clip);

        if(!_subscribedToCallback) {
            _source.End += _finished;
            _subscribedToCallback = true;
        }
    }
    public override void Pause(bool paused) {
        _paused = paused;
        if(_paused) {
            _pauseFrame = _source.Cursor;
            _source.Stop();
        }
        else {
            _source.Play(_clip);
            _source.Cursor = _pauseFrame;
        }
    }
    public override void TogglePause() {
        Pause(!_paused);
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
    public override float Volume() {
        return _source.Volume;
    }
    public MiniAudioExBackend() {
        _app = new AudioApp(SAMPLE_RATE, CHANNELS);
        _app.Loaded += _loaded;

        _audioThread = new Thread(() =>
        {
            _app.Run();
        })
        {
            IsBackground = true
        };

        _audioThread.Start();


        ShutdownHandler.OnShutdownCalled += _dispose;
    }
    private void _loaded() {
        _source = new AudioSource();
        _initialised = true;
    }
    private void _dispose()
    {
        if (!_initialised) return;

        _initialised = false;
        _audioThread.Join();
        AudioContext.Deinitialize();
    }
}