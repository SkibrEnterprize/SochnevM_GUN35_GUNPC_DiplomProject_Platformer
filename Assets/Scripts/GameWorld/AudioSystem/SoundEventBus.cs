using System;
using UnityEngine;

public class SoundEventBus : ISoundEventBus
{
    public event Action<SoundType, Vector3> OnSoundRequested;
    public void Play(SoundType type, Vector3 position = default)
        => OnSoundRequested?.Invoke(type, position);
}