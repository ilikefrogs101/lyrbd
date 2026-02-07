using System;
using System.Collections.Generic;
using ilikefrogs101.Logging;

namespace Lyrbd.Daemon;
public static class AudioHandler {
    public static AudioBackend _backend;
    
    private static bool _loop = false;
    private static bool _shuffle = false;
    private static bool _playing = false;

    private static int _queueIndex = 0;
    private static List<string> _queue;

    private static string _currentAddress;

    private static bool _initialised = false;
    private static void _initialise() {
        if(_initialised) return;

        _backend = new MiniAudioExBackend();
        _backend.OnCurrentTrackEnd += _finished;

        _initialised = true;
    }

    public static void Pause(bool paused) {
        if(!_playing) return;
        _backend.Pause(paused);
    }
    public static void TogglePause() {
        if(!_playing) return;
        _backend.TogglePause();
    }
    public static void Restart() {
        if(!_playing) return;
        _backend.Restart();
    }
    public static void Stop() {
        if(!_playing) return;
        _backend.Stop();
        _playing = false;
    }
    public static void Next() {
        if(!_playing) return;
        _pickNextTrack();
    }
    public static void Previous() {
        if(!_playing) return;
        _queueIndex -= 2;
        if(_queueIndex < 0) {
            _queueIndex = _queue.Count - 1;
        }
        _pickNextTrack();
    }
    public static void SkipQueue(int position) {
        if(!_playing) return;
        if(_queue.Count >= position) return;
        _queueIndex = position;
        _pickNextTrack();
    }
    public static void Forward(ulong seconds) {
        if(!_playing) return;
        _backend.Forward(seconds);
    }
    public static void Backward(ulong seconds) {
        if(!_playing) return;
        _backend.Backward(seconds);
    }
    public static void SetShuffle(bool shuffle) {
        if(!_playing) {
            _shuffle = shuffle;
            return;
        }
        int currentTrackIndex = _queueIndex - 1;
        if(currentTrackIndex < 0) {
            currentTrackIndex = _queue.Count;
        }
        string currentTrackID = _queue[currentTrackIndex];

        _shuffle = shuffle;
        
        _buildQueueFromSource();
        _queueIndex = _queue.IndexOf(currentTrackID) + 1;
    }
    public static void ToggleShuffle() {
        SetShuffle(!_shuffle);
    }
    public static void SetLoop(bool loop) {
        _loop = loop;
    }
    public static void ToggleLoop() {
        SetLoop(!_loop);
    }
    public static void SetVolume(float volumePercent) {
        _initialise();
        _backend.SetVolume(volumePercent / 100);
    }
    public static void Play(string address) {
        if(!TrackManager.ValidAddress(address)) {
            Log.ErrorMessage($"Cannot play {address}, it does not exist");
            return;
        }

        _initialise();
        
        _currentAddress = address;

        _buildQueueFromSource();
        _pickNextTrack();

        _playing = true;
    }

    private static void _buildQueueFromSource() {
        _queueIndex = 0;
        (string type, string id) = TrackManager.SplitAddress(_currentAddress);

        switch(type) {
            case "track":
                _queue = [_currentAddress.Split(':')[1]];
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

    public static string GetAddress() {
        if(_playing == false) return "Not Playing";
        return _currentAddress;
    }
    public static string GetCurrentTrackId() {
        if(_playing == false) return "Not Playing";
        return _backend.CurrentTrack;
    }
    public static ulong GetProgress() {
        if(_playing == false) return 0;
        return _backend.Progress();
    }
    public static ulong GetLength() {
        if(_playing == false) return 0;
        return _backend.Length();
    }
    public static string[] GetQueue() {
        if(_playing == false) return [];
        return [.. _queue];
    }
    public static int GetQueuePosition() {
        if(_playing == false) return 0;
        return _queueIndex - 1;
    }
    public static bool GetShuffleState() {
        return _shuffle;
    }
    public static bool GetLoopState() {
        return _loop;
    }
    public static float GetVolume() {
        if(!_initialised) return 100;
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