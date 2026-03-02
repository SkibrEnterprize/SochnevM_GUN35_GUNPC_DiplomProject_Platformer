using System.Linq;
using UnityEngine;
using Zenject;

public class TriggerForObject : MonoBehaviour
{
    [Tooltip("Список объектов, кот. реагируют на триггер. " +
        "Если не назначено в инспекторе - берутся дочерние объекты с компонентом FeaturesObject ")]
    [SerializeField] private FeaturesObject[] _featureObjects;

    private SignalBus _signalBus;

    [Inject]
    private void Construct(SignalBus signalBus)
    {
        _signalBus = signalBus;
    }

    private void Awake()
    {
        if (_featureObjects.Length == 0)
        {
            _featureObjects = GetComponentsInChildren<FeaturesObject>();
                Debug.Log("Features!!!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<CharacterController>(out CharacterController controller))
            foreach (var feature in _featureObjects)
            {
                feature?.SomeActions();
                Debug.Log("Color Is Changeed");
            }
    }
}

