using UnityEngine;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private AudioMixer _mixer;

    private const string MusicKey = "MusicVolume";
    private const string SfxKey = "SfxVolume";

    private const float DefaultVolume = 0.75f;

    private void Start()
    {
        ApplyStoredSettings();
    }

    public void SetMusicVolume(float volume)
    {
        _mixer.SetFloat("MusicVol", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat(MusicKey, volume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        _mixer.SetFloat("SFXVol", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat(SfxKey, volume); 
        PlayerPrefs.Save();
    }

    public float GetMusicVolume() => PlayerPrefs.GetFloat(MusicKey, DefaultVolume);
    public float GetSFXVolume() => PlayerPrefs.GetFloat(SfxKey, DefaultVolume);


    public void ApplyStoredSettings()
    {
        SetMusicVolume(GetMusicVolume());
        SetSFXVolume(GetSFXVolume());
    }
}