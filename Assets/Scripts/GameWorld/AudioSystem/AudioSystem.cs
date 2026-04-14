using UnityEngine;
using System.Collections.Generic;

public class AudioSystem : MonoBehaviour, IAudioSystem
{
    [Header("Pool Settings")]
    [SerializeField] private int poolSize = 10;
    [SerializeField] private Transform _poolParent;
    private List<AudioSource> _pool = new List<AudioSource>();

    [Header("Audio Settings")]
    [SerializeField] private UnityEngine.Audio.AudioMixerGroup _sfxGroup;

    [Range(0f, 1f)]
    [SerializeField] private float spatialBlend = 1f; // 0 = 2D, 1 = 3D
    [SerializeField] private float minDistance = 1f;
    [SerializeField] private float maxDistance = 20f;
    [SerializeField] private AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic;

    void Awake()
    {
        if (_poolParent == null) _poolParent = transform;
        for (int i = 0; i < poolSize; i++)
        {
            var go = new GameObject("Source_" + i);
            go.transform.SetParent(_poolParent);

            var s = go.AddComponent<AudioSource>();
            s.outputAudioMixerGroup = _sfxGroup;

            // Применяем настройки из инспектора
            s.spatialBlend = spatialBlend;
            s.minDistance = minDistance;
            s.maxDistance = maxDistance;
            s.rolloffMode = rolloffMode;
            s.dopplerLevel = 0;

            _pool.Add(s);
        }
    }

    public void Play(AudioEvent ev, Vector3 position)
    {
        var source = _pool.Find(s => !s.isPlaying);
        if (source != null)
        {
            source.transform.position = position;
            ev.Play(source);
        }


    }
}