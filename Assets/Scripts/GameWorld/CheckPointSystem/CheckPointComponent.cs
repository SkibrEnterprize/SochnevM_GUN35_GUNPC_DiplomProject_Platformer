using UnityEngine;
using Zenject;
using System.Threading.Tasks; // Нужно для Task

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
            // Мы не можем сделать OnTriggerEnter асинхронным напрямую, 
            // поэтому просто вызываем асинхронный метод без ожидания (Fire and Forget)
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

        Debug.Log($"[Checkpoint] Активирован: {gameObject.name}");
    }

    private async Task FadeInLightAsync()
    {
        _checkpointLight.gameObject.SetActive(true);
        _checkpointLight.intensity = 0;

        float currentTime = 0;

        // Используем токен отмены, чтобы задача прекратилась, если объект удалят
        var token = destroyCancellationToken;

        while (currentTime < _fadeDuration)
        {
            if (token.IsCancellationRequested) return;

            currentTime += Time.deltaTime;
            _checkpointLight.intensity = Mathf.Lerp(0, _targetIntensity, currentTime / _fadeDuration);

            // Аналог yield return null для async
            await Task.Yield();
        }

        if (_checkpointLight != null)
            _checkpointLight.intensity = _targetIntensity;
    }
}
//using Unity.VisualScripting;
//using UnityEngine;
//using UnityEngine.Experimental.GlobalIllumination;
//using Zenject;

//public class CheckPointComponent : MonoBehaviour
//{
//    private Light _checkpointLight;
//    [SerializeField] private bool _isActivate = false;
//    private CheckPointModel _checkPointModel;

//    public Vector3 Position => transform.position;
//    public Quaternion Rotation => transform.rotation;

//    [Inject]
//    private void Construct(CheckPointModel checkPointHandler)
//    {
//        _checkPointModel = checkPointHandler;
//    }
//    private void Awake()
//    {
//        _checkpointLight = GetComponentInChildren<Light>();
//    }
//    private void Start()
//    {

//        if (_checkpointLight == null)
//        {
//            Debug.Log($"[Checkpoint] {gameObject.name} не нашел Spot Light в дочерних объектах!");
//        }
//        UpdateVisual();
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        if (_isActivate) return;
//        if (other.gameObject.TryGetComponent<CharacterController>(out CharacterController controller))
//        {
//            _isActivate = true;
//            _checkPointModel.SetCheckpoint(transform.position, transform.rotation);
//            Debug.Log($"[Checkpoint] Активирован: {gameObject.name}");
//        UpdateVisual();
//        }
//    }

//    private void UpdateVisual()
//    {
//        if (_checkpointLight != null)
//        {
//            _checkpointLight.gameObject.SetActive(_isActivate);
//        }
//    }
//}