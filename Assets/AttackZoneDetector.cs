using System.Collections.Generic;
using UnityEngine;

public class AttackZoneDetector : MonoBehaviour
{
    private readonly List<IHealthAffected> _targetsInRange = new List<IHealthAffected>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IHealthAffected>(out var target))
        {
            _targetsInRange.Add(target);
            Debug.Log($"Цель {other.name} добавлена в зону атаки");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<IHealthAffected>(out var target))
        {
            _targetsInRange.Remove(target);
        }
    }

    public List<IHealthAffected> GetTargets()
    {
        _targetsInRange.RemoveAll(t => t == null || (t is MonoBehaviour mb && mb == null));
        return _targetsInRange;
    }
}