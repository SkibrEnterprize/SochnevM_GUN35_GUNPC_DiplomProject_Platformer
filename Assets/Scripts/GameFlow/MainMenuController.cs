using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MainMenuController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject _mainPanel;
    [SerializeField] private GameObject _levelSelectPanel;
    [SerializeField] private GameObject _settingsPanel;

    [Header("Audio Sliders")]
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _sfxSlider;

    private SceneLoader _sceneLoader;
    private SettingsManager _settingsManager;

    [Inject]
    public void Construct(SceneLoader sceneLoader, 
        SettingsManager settingsManager)
    {
        _sceneLoader = sceneLoader;
        _settingsManager = settingsManager;
    }
    private void Start()
    {
        _musicSlider.value = _settingsManager.GetMusicVolume();
        _sfxSlider.value = _settingsManager.GetSFXVolume();

        _musicSlider.onValueChanged.AddListener(val => _settingsManager.SetMusicVolume(val));
        _sfxSlider.onValueChanged.AddListener(val => _settingsManager.SetSFXVolume(val));

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        
    }

    public void OpenSettings()
    {
        _mainPanel.SetActive(false);
        _settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        _settingsPanel.SetActive(false);
        _mainPanel.SetActive(true);
    }
    public void OpenLevelSelect()
    {
        _mainPanel.SetActive(false);
        _levelSelectPanel.SetActive(true);
    }
    public void ExitLevelSelect()
    {
        _mainPanel.SetActive(true);
        _levelSelectPanel.SetActive(false);
    }

    public void CloseLevelSelect()
    {
        _levelSelectPanel.SetActive(false);
        _mainPanel.SetActive(true);
    }

    public void LoadLevelByName(string levelName)
    {
        Debug.Log($"Загрузка уровня: {levelName}");
        _sceneLoader.LoadLevel(levelName);
    }

    public void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}