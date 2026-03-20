using System.Collections.Generic;
using UnityEngine;
using Zenject;

[CreateAssetMenu(menuName = "Audio/Sound Library")]
public class SoundLibrary : ScriptableObject
{
    [Inject] private IAudioManager _audioManager;
    
    public List<SoundMapping> sounds;

    public void RequestPlay(SoundType type, Vector3 position = default)
    {
        var mapping = sounds.Find(s => s.type == type);
        if (mapping.audioEvent != null)
        {            
            _audioManager.Play(mapping.audioEvent, position);
        }
        else
        {
            Debug.Log($"Please choose sound for {type} in Main Sound Library configuration");
        }
    }
}
