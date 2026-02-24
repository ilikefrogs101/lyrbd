using System;
using System.Collections.Generic;

namespace Lyrbd.Daemon;
public static class QueueHandler {
    private static bool _loop;
    private static bool _shuffle;

    private static int _queueIndex;
    private static List<string> _queue = [];

    private static string _sourceAddress;
    private static string _trackId;

    public static void Play(string address) {
        if (!LibraryManager.ValidAddress(address)) return;

        _sourceAddress = address;

        _buildQueue();
        _pickNextTrack();
    }
    public static void Stop() {
        _sourceAddress = null;
        AudioHandler.Stop();
    }
    public static void Next() {
        if(_sourceAddress == null) return;

        _pickNextTrack();
    }
    public static void Previous() {      
        if(_sourceAddress == null) return;

        _queueIndex -= 2;
        if(_queueIndex < 0) {
            _queueIndex = _queue.Count - 1;
        }
        _pickNextTrack();
    }
    public static void SkipQueue(int position) {
        if (_sourceAddress == null) return;
        if (position >= _queue.Count || position < 0) return;

        _queueIndex = position;
        _pickNextTrack();    
    }
    public static void SetLoop(bool loop) {
        _loop = loop;
    }
    public static void SetShuffle(bool shuffle) {
        if (_sourceAddress == null) {
            _shuffle = shuffle;
            return;
        }

        int trackIndex = _queueIndex - 1;
        if (trackIndex < 0) {
            trackIndex = _queue.Count;
        }
        string trackId = _queue[trackIndex];

        _shuffle = shuffle;
        
        _buildQueue();
        _queueIndex = _queue.IndexOf(trackId) + 1;
    }

    public static bool IsLooped() {
        return _loop;
    }
    public static bool IsShuffled() {
        return _shuffle;
    }
    public static int QueuePosition() {
        return _queueIndex - 1;
    }
    public static string[] Queue() {
        return [.. _queue];
    }
    public static string Address() {
        return _sourceAddress;
    }
    public static string TrackId() {
        return _trackId;
    }

    public static void Initialise() {
        AudioHandler.OnCurrentTrackEnd += _pickNextTrack;
    }

    private static void _buildQueue() {
        _queueIndex = 0;
        _queue = [.. LibraryManager.TracksFromAddress(_sourceAddress)];

        if (_shuffle) {
            _shuffleQueue();
        }
    }
    private static void _pickNextTrack() {
        if (_queueIndex == _queue.Count) {
            if (_loop) {
                _buildQueue();
            }
            else {
                Stop();
                return;
            }
        }
        
        _trackId = _queue[_queueIndex];
        AudioHandler.Play(_trackId);
        ++_queueIndex;
    }

    private static void _shuffleQueue() {
        int n = _queue.Count;
        while (n > 1) {
            n--;
            int k = Random.Shared.Next(n + 1);
            string value = _queue[k];
            _queue[k] = _queue[n];
            _queue[n] = value;
        }
    }
}