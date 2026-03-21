using System;
using UnityEngine;
using Zenject;

public class SoundEffectObserver : IInitializable, IDisposable
{
    private readonly SoundLibrary _soundLibrary;
    private readonly ISoundEventBus _eventBus;
    private readonly IAudioManager _audioManager;

    public SoundEffectObserver(SoundLibrary soundLibrary, ISoundEventBus eventBus, IAudioManager audioManager)
    {
        _soundLibrary = soundLibrary;
        _eventBus = eventBus;
        _audioManager = audioManager;
    }

    public void Initialize()
    {
        _eventBus.OnSoundRequested += HandleSoundRequest;
    }

    private void HandleSoundRequest(SoundType type, Vector3 position)
    {
        var audioEvent = _soundLibrary.GetEvent(type);
        if (audioEvent != null)
        {
            _audioManager.Play(audioEvent, position);
        }
        else
        {
            Debug.LogWarning($"Sound for {type} not found in library!");
        }
    }

    public void Dispose()
    {
        _eventBus.OnSoundRequested -= HandleSoundRequest;
    }
}
