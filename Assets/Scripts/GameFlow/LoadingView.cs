using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

public class LoadingView : MonoBehaviour
{
    [SerializeField] private GameObject _canvasRoot; 
    [SerializeField] private Slider _progressBar;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private TextMeshProUGUI _progressText;
    [SerializeField] float _fadeDuration = 0.5f;

    public async Task FadeIn()
    {
        _canvasRoot.SetActive(true);
        _canvasGroup.alpha = 0;
        while (_canvasGroup.alpha < 1)
        {
            _canvasGroup.alpha += Time.unscaledDeltaTime / _fadeDuration;
            await Task.Yield();
        }
    }

    public async Task FadeOut()
    {
        while (_canvasGroup.alpha > 0)
        {
            _canvasGroup.alpha -= Time.unscaledDeltaTime / _fadeDuration;
            await Task.Yield();
        }
        _canvasRoot.SetActive(false);
    }

    public void UpdateProgress(float progress)
    {
        StopAllCoroutines(); 
        StartCoroutine(SmoothProgress(progress));
    }

    private System.Collections.IEnumerator SmoothProgress(float target)
    {
        while (!Mathf.Approximately(_progressBar.value, target))
        {
            _progressBar.value = Mathf.MoveTowards(_progressBar.value, target, Time.unscaledDeltaTime * 2f);
            yield return null;
        }
    }
    public void InitialHide()
    {
        _canvasGroup.alpha = 0;
        _canvasRoot.SetActive(false);
    }
}