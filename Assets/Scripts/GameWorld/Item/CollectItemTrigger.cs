using UnityEngine;
using Zenject;

public class CollectItemTrigger : MonoBehaviour
{
    [SerializeField] private int _valueForCollect = 1;
    private CollectItemModel _collectModel;

    [Inject]
    private void Construct(CollectItemModel itemModel)
    {
        _collectModel = itemModel;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<CharacterController>(out CharacterController controller)
            && isActiveAndEnabled)
        {
          gameObject.SetActive(false);
          _collectModel.CollectItem(_valueForCollect);
        }
    }
}
