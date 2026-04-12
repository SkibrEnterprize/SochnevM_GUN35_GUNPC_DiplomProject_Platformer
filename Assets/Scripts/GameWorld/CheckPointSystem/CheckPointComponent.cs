using UnityEngine;
using Zenject;
using System.Threading.Tasks;

public class CheckPointComponent : MonoBehaviour
{
    private Light _checkpointLight;
    [SerializeField] private bool _isActivate = false;

    [Header("Light Settings")]
    [SerializeField] private float _targetIntensity = 5f;
    [SerializeField] private float _fadeDuration = 1.5f;

    private CheckPointModel _checkPointModel;

    [Inject]
    private void Construct(CheckPointModel checkPointHandler)
    {
        _checkPointModel = checkPointHandler;
    }

    private void Awake()
    {
        _checkpointLight = GetComponentInChildren<Light>(true);
    }

    private void Start()
    {
        if (_checkpointLight != null)
        {
            _checkpointLight.intensity = _isActivate ? _targetIntensity : 0;
            _checkpointLight.gameObject.SetActive(_isActivate);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isActivate) return;

        if (other.gameObject.TryGetComponent<CharacterController>(out _))
        {
            ActivateCheckpointAsync();
        }
    }

    private async void ActivateCheckpointAsync()
    {
        _isActivate = true;
        _checkPointModel.SetCheckpoint(transform.position, transform.rotation);

        if (_checkpointLight != null)
        {
            await FadeInLightAsync();
        }
    }

    private async Task FadeInLightAsync()
    {
        _checkpointLight.gameObject.SetActive(true);
        _checkpointLight.intensity = 0;

        float currentTime = 0;

        var token = destroyCancellationToken;

        while (currentTime < _fadeDuration)
        {
            if (token.IsCancellationRequested) return;

            currentTime += Time.deltaTime;
            _checkpointLight.intensity = Mathf.Lerp(0, _targetIntensity, currentTime / _fadeDuration);

            await Task.Yield();
        }

        if (_checkpointLight != null)
            _checkpointLight.intensity = _targetIntensity;
    }
}
