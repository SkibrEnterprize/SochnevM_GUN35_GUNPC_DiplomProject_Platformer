using System;
using UnityEngine;

public interface ISoundEventBus
{
    event Action<SoundType, Vector3> OnSoundRequested;
    void Play(SoundType type, Vector3 position = default);
}