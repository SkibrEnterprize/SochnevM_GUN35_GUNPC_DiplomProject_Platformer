using System.Threading.Tasks;
using UnityEngine;

public class TutorialPointTrigger : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _targetIntensity = 1.0f;
    [SerializeField] private float _fadeDuration = 0.5f;
    [SerializeField] private float _bobSpeed = 2f;      // Скорость покачивания
    [SerializeField] private float _bobAmount = 0.1f;   // Амплитуда (высота)

    private Light _checkpointLight;
    private CanvasGroup _canvasGroup;
    private Vector3 _initialCanvasPos;
    private bool _isFadingIn; // Флаг для предотвращения конфликтов задач

    private void Awake()
    {
        _canvasGroup = GetComponentInChildren<CanvasGroup>();
        _checkpointLight = GetComponentInChildren<Light>(true);

        if (_canvasGroup != null)
            _initialCanvasPos = _canvasGroup.transform.localPosition;
    }

    private void Start()
    {
        if (_canvasGroup != null) _canvasGroup.alpha = 0;

        if (_checkpointLight != null)
        {
            _checkpointLight.intensity = 0;
            _checkpointLight.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (_canvasGroup != null && _canvasGroup.alpha > 0.01f)
        {
            float newY = _initialCanvasPos.y + Mathf.Sin(Time.time * _bobSpeed) * _bobAmount;
            _canvasGroup.transform.localPosition = new Vector3(_initialCanvasPos.x, newY, _initialCanvasPos.z);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<CharacterController>(out _))
        {
            _isFadingIn = true;
            _ = FadeAsync(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent<CharacterController>(out _))
        {
            _isFadingIn = false;
            _ = FadeAsync(false);
        }
    }

    private async Task FadeAsync(bool fadeIn)
    {
        if (_canvasGroup == null || _checkpointLight == null) return;

        float startAlpha = _canvasGroup.alpha;
        float targetAlpha = fadeIn ? 1 : 0;

        float startIntensity = _checkpointLight.intensity;
        float targetIntensity = fadeIn ? _targetIntensity : 0;

        if (fadeIn) _checkpointLight.gameObject.SetActive(true);

        float currentTime = 0;
        var token = destroyCancellationToken;

        while (currentTime < _fadeDuration)
        {
            if (token.IsCancellationRequested) return;

            if (_isFadingIn != fadeIn) return;

            currentTime += Time.deltaTime;
            float t = currentTime / _fadeDuration;

            _canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            _checkpointLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, t);

            await Task.Yield();
        }

        if (_isFadingIn == fadeIn)
        {
            _canvasGroup.alpha = targetAlpha;
            _checkpointLight.intensity = targetIntensity;

            // Если выключали — деактивируем свет в самом конце
            if (!fadeIn) _checkpointLight.gameObject.SetActive(false);
        }
    }
}
