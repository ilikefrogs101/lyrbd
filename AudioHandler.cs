using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;

namespace ilikefrogs101.MusicPlayer;
public static class AudioHandler {
    public static AudioBackend _backend;
    
    private static bool _loop = false;
    private static bool _shuffle = false;

    private static int _queueIndex = 0;
    private static List<string> _queue;

    private static string _playSource;

    private static bool _initialised = false;
    private static void _initialise() {
        if(_initialised) return;

        _backend = new MiniAudioExBackend();
        _backend.OnCurrentTrackEnd += _finished;

        _initialised = true;
    }

    
    public static void Pause() {
        _backend.Pause();
    }
    public static void Resume() {
        _backend.Resume();
    }
    public static void Restart() {
        _backend.Restart();
    }
    public static void Stop() {
        _backend.Stop();
    }
    public static void Next() {
        _pickNextTrack();
    }
    public static void Previous() {
        _queueIndex -= 2;
        if(_queueIndex < 0) {
            _queueIndex = _queue.Count - 1;
        }
        _pickNextTrack();
    }
    public static void Forward(ulong seconds) {
        _backend.Forward(seconds);
    }
    public static void Backward(ulong seconds) {
        _backend.Backward(seconds);
    }
    public static void SetShuffle(bool shuffle) {
        int currentTrackIndex = _queueIndex - 1;
        if(currentTrackIndex < 0) {
            currentTrackIndex = _queue.Count;
        }
        string currentTrackID = _queue[currentTrackIndex];

        _shuffle = shuffle;
        
        _buildQueueFromSource();
        _queueIndex = _queue.IndexOf(currentTrackID) + 1;
    }
    public static void SetLoop(bool loop) {
        _loop = loop;
    }
    public static void SetVolume(float volumePercent) {
        _backend.SetVolume(volumePercent / 100);
    }
    public static void Play(string id) {
        _initialise();

        _playSource = id;

        _buildQueueFromSource();
        _pickNextTrack();
    }

    private static void _buildQueueFromSource() {
        string type = _playSource.Split(':')[0];
        string id = _playSource.Split(':')[1];

        switch(type) {
            case "track":
                _queue = [_playSource.Split(':')[1]];
                break;
            case "playlist":
                _queue = [.. TrackManager.GetPlaylist(id).Tracks];
                break;
            case "artist":
                _queue = [.. TrackManager.GetArtist(id).Tracks];
                break;
            case "album":
                _queue = [.. TrackManager.GetAlbum(id).Tracks];
                break;
        }

        if (_shuffle) {
            _queue.Shuffle();
        }
    }
    private static void _pickNextTrack() {
        if(_queueIndex == _queue.Count) {
            if(_loop) {
                _buildQueueFromSource();
                _queueIndex = 0;
            }
            else {
                Stop();
                return;
            }
        }

        _play(_queue[_queueIndex]);
        ++_queueIndex;
    }
    private static void _play(string track) {
        _backend.Play(track);
    }
    private static void _finished() {
        _pickNextTrack();
    }

    public static string GetSource() {
        return _playSource;
    }
    public static string GetCurrentTrackId() {
        return _backend.CurrentTrack;
    }
    public static ulong GetProgress() {
        return _backend.Progress();
    }
    public static ulong GetLength() {
        return _backend.Length();
    }
    public static string[] GetQueue() {
        return [.. _queue];
    }
    public static int GetQueuePosition() {
        return _queueIndex;
    }
    public static bool GetShuffleState() {
        return _shuffle;
    }
    public static bool GetLoopState() {
        return _loop;
    }
    public static float GetVolume() {
        return _backend.Volume() * 100;
    }
    
    private static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1) {
            n--;
            int k = Random.Shared.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}