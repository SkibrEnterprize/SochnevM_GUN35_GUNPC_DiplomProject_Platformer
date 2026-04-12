using UnityEngine;
using DG.Tweening;

public class RotationScript : MonoBehaviour
{
    public enum RotationAxis { X, Y, Z }

    public RotationAxis rotationAxis = RotationAxis.Y;
    public float rotationSpeed = 50.0f; // Градусов в секунду

    private void Start()
    {
        // Определяем вектор оси
        Vector3 axisVector = rotationAxis switch
        {
            RotationAxis.X => Vector3.right,
            RotationAxis.Y => Vector3.up,
            RotationAxis.Z => Vector3.forward,
            _ => Vector3.up
        };

        float duration = 360f / rotationSpeed;

        transform.DORotate(axisVector * 360f, duration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)        
            .SetLoops(-1, LoopType.Incremental); 
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }
}