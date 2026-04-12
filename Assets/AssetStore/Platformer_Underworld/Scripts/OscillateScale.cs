using UnityEngine;
using DG.Tweening;

public class OscillateScale : MonoBehaviour
{
    public float scaleFactor = 1.2f; 
    public float duration = 2f;
    public bool useRandomDelay = false;
    public float maxRandomDelay = 1f;

    private void Start()
    {
        Vector3 targetScale = transform.localScale * scaleFactor;

        var tween = transform.DOScale(targetScale, duration / 2f)
            .SetEase(Ease.InOutQuad)
            .SetLoops(-1, LoopType.Yoyo);

        if (useRandomDelay)
        {
            float delay = Random.Range(0f, maxRandomDelay);
            tween.SetDelay(delay);
        }
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }
}