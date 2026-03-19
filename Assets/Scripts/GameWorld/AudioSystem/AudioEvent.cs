using UnityEngine;

[CreateAssetMenu(menuName = "Audio/Audio Event")]
public class AudioEvent : ScriptableObject
{
    public AudioClip[] clips;
    [Range(0, 1)] public float volume = 1f;
    [Range(0.1f, 2f)] public float pitch = 1f;

    public void Play(AudioSource source)
    {
        if (clips.Length == 0) return;
        source.clip = clips[Random.Range(0, clips.Length)];
        source.volume = volume;
        source.pitch = pitch;
        source.Play();
    }
}