using DG.Tweening;
using UnityEngine;
using Zenject;

public class TriggerChangeHealthComponent : MonoBehaviour
{
    [SerializeField] private int _amountOfChange = 10;
    [SerializeField] private bool _isReusable = false;

    private VFXEventBus _vfxBus;
    private bool _isUsed; // Для одноразовых объектов

    [Inject]
    private void Construct(VFXEventBus vFXEventBus) => _vfxBus = vFXEventBus;

    private void OnTriggerEnter(Collider other)
    {
        if (_isUsed) return;

        if (other.gameObject.TryGetComponent<IHealthAffected>(out IHealthAffected healthAffected))
        {
            healthAffected.ApplyHealthChange(_amountOfChange);

            // Визуал
            PlayVFX();

            if (!_isReusable)
            {
                _isUsed = true;
                PlayCollectAnimation();
            }
            else
            {
                // Эффект "пульсации" для многоразового объекта, чтобы видно было активацию
                transform.DOPunchScale(Vector3.one * 0.1f, 0.2f);
            }
        }
    }

    private void PlayVFX()
    {
        var vfxType = _amountOfChange > 0 ? VFXType.Healing : VFXType.Hit;
        _vfxBus.Play(vfxType, FeetPosition, 0.5f);
    }

    private void PlayCollectAnimation()
    {
        if (TryGetComponent<Collider>(out var c)) c.enabled = false;

        transform.DOMoveY(transform.position.y + 1.2f, 0.4f).SetEase(Ease.OutQuad);
        transform.DOScale(Vector3.zero, 0.4f)
            .SetEase(Ease.InBack)
            .OnComplete(() => Destroy(gameObject));
    }

    private Vector3 FeetPosition => TryGetComponent<Collider>(out var c)
        ? new Vector3(transform.position.x, c.bounds.min.y, transform.position.z)
        : transform.position;
}

//using DG.Tweening;
//using UnityEngine;
//using Zenject; // Не забудь добавить пространство имен

//public class TriggerChangeHealthComponent : MonoBehaviour
//{
//    [SerializeField] private int _amountOfChange = 10;
//    [SerializeField] private bool _isReusable = false;
//    private VFXEventBus _vfxBus;

//    private bool _isUsed; // Флаг, чтобы не сработало дважды

//    [Inject]
//    private void Construct(VFXEventBus vFXEventBus)
//    { _vfxBus = vFXEventBus; }

//    private void OnTriggerEnter(Collider other)
//    {

//        if (_isUsed) return;

//        if (other.gameObject.TryGetComponent<IHealthAffected>(out IHealthAffected healthAffected))
//        {
//            _isUsed = true; // Сразу блокируем повторное использование

//                healthAffected.ApplyHealthChange(_amountOfChange);

//            bool isHealing = _amountOfChange > 0;
//            if (isHealing)
//            {
//                _vfxBus.Play(VFXType.Healing, FeetPosition, 0.5f);
//                PlayCollectAnimation();
//            }
//            else
//            {

//            }
//        }
//    }

//    private void PlayCollectAnimation()
//    {
//        // 1. Выключаем коллайдер, чтобы объект больше не взаимодействовал ни с чем
//        if (TryGetComponent<Collider>(out var c)) c.enabled = false;

//        // 2. Анимация DOTween:
//        // Подпрыгивает вверх на 1 единицу, уменьшается в 0 и удаляется по завершении
//        transform.DOMoveY(transform.position.y + 1.2f, 0.4f).SetEase(Ease.OutQuad);
//        transform.DOScale(Vector3.zero, 0.4f)
//            .SetEase(Ease.InBack)
//            .OnComplete(() => Destroy(gameObject)); // Удаляем после анимации
//    }

//    private Vector3 FeetPosition => TryGetComponent<Collider>(out var c)
//        ? new Vector3(transform.position.x, c.bounds.min.y, transform.position.z)
//        : transform.position;
//}

