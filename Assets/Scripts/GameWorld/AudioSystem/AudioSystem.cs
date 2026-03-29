using UnityEngine;
using System.Collections.Generic;

public class AudioSystem : MonoBehaviour, IAudioSystem
{
    [SerializeField] private int poolSize = 10;
    private List<AudioSource> _pool = new List<AudioSource>();

    void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            var s = new GameObject("Source_" + i).AddComponent<AudioSource>();
            s.transform.SetParent(transform);
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