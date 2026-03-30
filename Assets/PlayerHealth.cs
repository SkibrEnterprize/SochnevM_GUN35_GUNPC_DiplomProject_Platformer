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
    public void ApplyHealthChange(int delta, Vector3 sourcePosition = default)
    {
        _healthComponent.ApplyHealthChange(delta);
    }
}
