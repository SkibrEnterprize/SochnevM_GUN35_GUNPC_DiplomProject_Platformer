using UnityEngine;

public class FeaturesObject : MonoBehaviour
{
    [SerializeField] private bool _isOnceActivate = false;

    private Color _originalColor;
    private MeshRenderer _meshRenderer;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _originalColor = _meshRenderer.material.color;
    }

    public void SomeActions()
    {
        if (!_isOnceActivate)
        {
            ChangeColorRandomly();
            _isOnceActivate = true;
        }
        else
        {
            ResetColor();
            _isOnceActivate = false;
        }

    }

    private void ChangeColorRandomly()
    {       
            _meshRenderer.material.color = Random.ColorHSV();       
    }

    private void ResetColor()
    {
        _meshRenderer.material.color= _originalColor;
    }

}
