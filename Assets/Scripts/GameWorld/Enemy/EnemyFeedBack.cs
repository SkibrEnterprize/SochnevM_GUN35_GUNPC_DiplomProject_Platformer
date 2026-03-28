using DG.Tweening;
using UnityEngine;

public class EnemyFeedback : MonoBehaviour
{
    private EnemyHealth _health;
    private Material _material;
    private Color _originalColor;

    private void Awake()
    {
        _health = GetComponent<EnemyHealth>();
        _material = GetComponentInChildren<Renderer>().material;
        _originalColor = _material.color;
    }

    private void OnEnable() => _health.OnTakeDamage += PlayHitEffect;
    private void OnDisable() => _health.OnTakeDamage -= PlayHitEffect;

    private void PlayHitEffect(float damage)
    {
        // Вспышка красным через DOTween
        _material.DOColor(Color.red, 0.1f).OnComplete(() => _material.DOColor(_originalColor, 0.1f));

        // Маленькая тряска
        transform.DOShakePosition(0.2f, 0.2f);
    }
}