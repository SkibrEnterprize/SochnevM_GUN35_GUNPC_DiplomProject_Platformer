using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Audio/Sound Library")]
public class SoundLibrary : ScriptableObject
{
    public List<SoundMapping> sounds;

    public AudioEvent GetEvent(SoundType type)
    {
        var mapping = sounds.Find(s => s.type == type);
        if (mapping.audioEvent == null)
            Debug.LogWarning($"Sound for {type} not found in library!");

        return mapping.audioEvent;
    }
}