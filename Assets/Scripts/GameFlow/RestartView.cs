using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RestartView : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Slider _progressSlider;
    [SerializeField] private TextMeshProUGUI _label;
    private void Awake() => Hide();
    public void Show()
    {
        _canvasGroup.alpha = 1f;
        _progressSlider.value = 0;
    }
    public void UpdateProgress(float value)
    {
        _progressSlider.value = value;
    }
    public void Hide()
    {
        _canvasGroup.alpha = 0f;
        _progressSlider.value = 0;
    }
}