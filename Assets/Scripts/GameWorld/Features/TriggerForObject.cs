using UnityEngine;

public class TriggerForObject : MonoBehaviour
{
    [Tooltip("Список объектов, кот. реагируют на триггер. " +
        "Если не назначено в инспекторе - берутся дочерние объекты с компонентом FeaturesObject ")]
    [SerializeField] private FeaturesObject[] _featureObjects;

    private void Awake()
    {
        if (_featureObjects.Length == 0)
        {
            _featureObjects = GetComponentsInChildren<FeaturesObject>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<CharacterController>(out CharacterController controller))
            foreach (var feature in _featureObjects)
            {
                feature?.SomeActions();
            }
    }
}

