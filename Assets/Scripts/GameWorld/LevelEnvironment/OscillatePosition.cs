
    using UnityEngine;
    using DG.Tweening;

    public class OscillatePosition : MonoBehaviour
    {
        public Vector3 moveAxis = Vector3.up;
        public float moveDistance = 2f;
        public float duration = 2f;
        public bool useRandomDelay = false;
        public float maxRandomDelay = 1f;

        private void Start()
        {
            Vector3 targetPosition = transform.position + moveAxis.normalized * moveDistance;

            var tween = transform.DOMove(targetPosition, duration / 2f)
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

