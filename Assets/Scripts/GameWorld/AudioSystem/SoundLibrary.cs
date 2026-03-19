using System.Collections.Generic;
using UnityEngine;
using Zenject;

[CreateAssetMenu(menuName = "Audio/Sound Library")]
public class SoundLibrary : ScriptableObject
{
    [Inject] private IAudioManager _audioManager; // Прямая ссылка на сервис
    
    public List<SoundMapping> sounds;

    public void RequestPlay(SoundType type, Vector3 position = default)
    {
        var mapping = sounds.Find(s => s.type == type);
        if (mapping.audioEvent != null)
        {
            // Вызываем метод сервиса напрямую
            _audioManager.Play(mapping.audioEvent, position);
        }
    }
}
