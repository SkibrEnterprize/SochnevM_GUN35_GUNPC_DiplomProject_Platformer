
using UnityEngine;
using DG.Tweening;
using Zenject;
using Player;

public class ResizeZoneTrigger : MonoBehaviour
{
    [Range(0.1f, 2f)]
    public float TargetObjectScale = 1.0f;
    public bool ResetToDefaultScale;

    private float _defaultHeight;
    private float _defaultRadius;
    private PlayerStartParameters _playerStartParameters;
    private PlayerMovementSystem _movementComponent;
    private float _duration = 1f;

    [Inject]
    private void Construct(PlayerStartParameters playerStartParameters,
        PlayerMovementSystem movementComponent)
    {
        _playerStartParameters = playerStartParameters;
        _movementComponent = movementComponent;
        _defaultHeight = _playerStartParameters.DefaultHeight;
        _defaultRadius = _playerStartParameters.DefaultRadius;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ищем контроллер на родителе
        if (!other.TryGetComponent<CharacterController>(out var controller)) return;

        // Ищем меш (ребенка)
        if (controller.transform.childCount == 0) return;
        Transform visual = controller.transform.GetChild(0);

        float finalScale = ResetToDefaultScale ? 1.0f : TargetObjectScale;

        // Убиваем старые твины
        DOTween.Kill(controller);
        DOTween.Kill(visual);

        // 1. Анимируем числовое значение масштаба
        DOTween.To(() => visual.localScale.x, x =>
        {
            // 2. Скейлим визуальную модель (капсулу-ребенка)
            visual.localScale = Vector3.one * x;

            // 3. Синхронизируем физику (CharacterController на родителе)
            float newHeight = _defaultHeight * x;
            float newRadius = _defaultRadius * x;

            controller.height = newHeight;
            controller.radius = newRadius;

            // 4. КОРРЕКЦИЯ ЦЕНТРА (Важно!)
            // Чтобы родитель "стоял" на полу (y=0), а капсула росла вверх,
            // ее центр должен быть равен половине высоты.
            controller.center = new Vector3(0, newHeight / 2f, 0);

            // 5. КОРРЕКЦИЯ ВИЗУАЛА (так как у стандартной капсулы Pivot в центре)
            // Мы поднимаем ребенка так, чтобы его низ совпал с низом родителя.
            visual.localPosition = new Vector3(0, newHeight / 2f, 0);

        }, finalScale, _duration).SetTarget(controller).SetEase(Ease.Linear);
    }
    //if (!other.gameObject.TryGetComponent<CharacterController>(out CharacterController controller)) return;
    //other.transform.localScale = Vector3.one * _targetObjectScale;
}


