using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PauseMenuView : MonoBehaviour
{
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private GameObject _settingsPanel;

    [Header("Audio Sliders")]
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _sfxSlider;

    private GameManager _gameManager;
    private SceneLoader _sceneLoader;
    private SettingsManager _settingsManager;

    public Slider SfxSlider { get => _sfxSlider; set => _sfxSlider = value; }

    [Inject]
    public void Construct(GameManager gameManager, SceneLoader sceneLoader, SettingsManager settingsManager)
    {
        _gameManager = gameManager;
        _sceneLoader = sceneLoader;
        _settingsManager = settingsManager;
    }

    private void Start()
    {
        _musicSlider.value = _settingsManager.GetMusicVolume();
        _sfxSlider.value = _settingsManager.GetSFXVolume();

        _musicSlider.onValueChanged.AddListener(val => _settingsManager.SetMusicVolume(val));
        SfxSlider.onValueChanged.AddListener(val => _settingsManager.SetSFXVolume(val));
                
    }

    private void OnEnable() => _gameManager.OnStateChanged += HandleStateChange;
    private void OnDisable() => _gameManager.OnStateChanged -= HandleStateChange;

    private void HandleStateChange(GameState state)
    {
        bool isPaused = (state == GameState.Paused);
        _pausePanel.SetActive(isPaused);

        if (!isPaused) _settingsPanel.SetActive(false);

        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isPaused;
    }

    public void OpenSettings()
    {
        _pausePanel.SetActive(false);
        _settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        _settingsPanel.SetActive(false);
        _pausePanel.SetActive(true);
    }

    public void OnResumeClicked() => _gameManager.UpdateState(GameState.Playing);

    public void OnExitToMenuClicked()
    {
        Time.timeScale = 1f;
        _sceneLoader.LoadLevel("MainMenu");
    }
}