using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] private Slider _musicSlider;

    private SettingsManager _settingsManager;

    [Inject]
    public void Construct(SettingsManager settingsManager) => _settingsManager = settingsManager;

    private void Start()
    {
        _musicSlider.onValueChanged.AddListener(val => _settingsManager.SetMusicVolume(val));
    }
}