using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private float _speed = 10f;
    [SerializeField] private int _damage = -10;
    [SerializeField] private float _lifetime = 3f;

    public void Launch(Vector3 direction)
    {
        GetComponent<Rigidbody>().velocity = direction * _speed;
        Destroy(gameObject, _lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IHealthAffected target))
        {
            target.ApplyHealthChange(_damage, transform.position);
            Destroy(gameObject);
        }
        // Удали пулю при столкновении со стеной
        else if (((1 << other.gameObject.layer) & LayerMask.GetMask("Ground")) != 0)
        {
            Destroy(gameObject);
        }
    }
}
