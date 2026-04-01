using System.Collections;
using UnityEngine;

using UnityEngine.SceneManagement;
using Zenject;

public class MusicManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private MusicConfig _config;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private float _fadeDuration = 1.5f;
    private SettingsManager _settingsManager;

    private Coroutine _fadeCoroutine;

    [Inject]
    public void Construct(SettingsManager settingsManager) => _settingsManager = settingsManager;
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (_config == null) return;
        _settingsManager.ApplyStoredSettings();

        AudioClip clip = _config.GetTrackByBuildIndex(scene.buildIndex);
        PlayMusic(clip);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (_audioSource.clip == clip && _audioSource.isPlaying) return;

        if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
        _fadeCoroutine = StartCoroutine(FadeTo(clip));
    }

    private IEnumerator FadeTo(AudioClip newClip)
    {
        if (_audioSource.isPlaying)
        {
            while (_audioSource.volume > 0)
            {
                _audioSource.volume -= Time.deltaTime / _fadeDuration;
                yield return null;
            }
            _audioSource.Stop();
        }

        _audioSource.clip = newClip;

        if (newClip != null)
        {
            _audioSource.Play();
            while (_audioSource.volume < 1)
            {
                _audioSource.volume += Time.deltaTime / _fadeDuration;
                yield return null;
            }
        }
    }
}