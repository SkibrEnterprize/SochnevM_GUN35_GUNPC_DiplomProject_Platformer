using UnityEngine;

public class ResizeZoneTrigger : MonoBehaviour
{    
    [Range(0.5f, 1f)]
    [SerializeField] float _objectScale = 1.0f;   

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.TryGetComponent<CharacterController>(out CharacterController controller)) return;
        other.transform.localScale = Vector3.one * _objectScale;
    }

}
