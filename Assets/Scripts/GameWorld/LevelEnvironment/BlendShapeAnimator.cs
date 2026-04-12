
using UnityEngine;
using DG.Tweening; 

public class BlendShapeAnimator : MonoBehaviour
{
    private SkinnedMeshRenderer skinnedMeshRenderer;
    public int blendShapeIndex = 0;
    public float maxBlendShapeValue = 100f;
    public float animationSpeed = 1f;
    public AnimationCurve animationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private void Awake()
    {
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        if (skinnedMeshRenderer == null)
        {
            skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        }
    }

    private void Start()
    {
        if (skinnedMeshRenderer != null)
        {
            StartAnimation();
        }
        else
        {
            Debug.LogError("SkinnedMeshRenderer not found!");
        }
    }

    private void StartAnimation()
    {
        float duration = 1f / animationSpeed;

        DOTween.To(() => skinnedMeshRenderer.GetBlendShapeWeight(blendShapeIndex),
                   x => skinnedMeshRenderer.SetBlendShapeWeight(blendShapeIndex, x),
                   maxBlendShapeValue,
                   duration)
            .SetEase(animationCurve) 
            .SetLoops(-1, LoopType.Yoyo); 
    }

    private void OnDestroy()
    {
        skinnedMeshRenderer.DOKill();
    }}

