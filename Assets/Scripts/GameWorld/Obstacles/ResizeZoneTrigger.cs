
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
        if (!other.TryGetComponent<CharacterController>(out var controller)) return;

        if (controller.transform.childCount == 0) return;
        Transform visual = controller.transform.GetChild(0);

        float finalScale = ResetToDefaultScale ? 1.0f : TargetObjectScale;

        DOTween.Kill(controller);
        DOTween.Kill(visual);

        DOTween.To(() => visual.localScale.x, x =>
        {
            visual.localScale = Vector3.one * x;

            float newHeight = _defaultHeight * x;
            float newRadius = _defaultRadius * x;

            controller.height = newHeight;
            controller.radius = newRadius;

            controller.center = new Vector3(0, newHeight / 2f, 0);

            visual.localPosition = new Vector3(0, newHeight / 2f, 0);

        }, finalScale, _duration).SetTarget(controller).SetEase(Ease.Linear);
    }
}


