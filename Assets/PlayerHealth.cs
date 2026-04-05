using UnityEngine;
using Zenject;

public class PlayerHealth : MonoBehaviour, IHealthAffected
{
    private IHealthAffected _healthComponent;

    [Inject]
    private void Construct(IHealthAffected healthComponent)
    {
        _healthComponent = healthComponent;
    }

    public void ApplyHealthChange(int delta, Vector3 sourcePosition = default,
                              DamageType type = DamageType.Default, float knockbackForce = 0f)
    {
        _healthComponent.ApplyHealthChange(delta, sourcePosition, type, knockbackForce);
    }
}
