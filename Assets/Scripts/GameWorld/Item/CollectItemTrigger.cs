using UnityEngine;
using Zenject;

public class CollectItemTrigger : MonoBehaviour
{
    private CollectItemModel _itemModel;

    [Inject]
    private void Construct(CollectItemModel itemModel)
    {
        _itemModel = itemModel;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<CharacterController>(out CharacterController controller)
            && isActiveAndEnabled)
        {
          gameObject.SetActive(false);
            _itemModel.CollectItem();
        }
    }
}
