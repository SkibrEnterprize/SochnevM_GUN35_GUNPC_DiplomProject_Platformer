using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private float _speed = 15f; 
    [SerializeField] private int _damage = -10;
    [SerializeField] private float _lifetime = 3f;

    private Vector3 _originPosition; 

    public void Launch(Vector3 direction, Vector3 shooterPosition)
    {
        _originPosition = shooterPosition; 

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = direction * _speed;
        }

        transform.right = direction;

        Destroy(gameObject, _lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IHealthAffected target))
        {
            target.ApplyHealthChange(_damage, _originPosition);
            Destroy(gameObject);
        }
        else if (((1 << other.gameObject.layer) & LayerMask.GetMask("Ground", "Wall")) != 0)
        {
            Destroy(gameObject);
        }
    }
}
