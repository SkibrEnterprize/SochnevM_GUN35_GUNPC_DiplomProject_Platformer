using UnityEngine;

public class PlayerStartParameters
{
    private CharacterController _characterController;
    private float _defaultHeight;
    private float _defaultRadius;
    private Transform _viewTransform;

    public float DefaultHeight => _defaultHeight;
    public float DefaultRadius => _defaultRadius;
    public Transform ViewTransform => _viewTransform; 
    public PlayerStartParameters(CharacterController controller)
    {
        _characterController = controller;
        _defaultHeight = _characterController.height;
        _defaultRadius = _characterController.radius;
        Debug.Log($"_defaultHeight = {_defaultHeight}");
        Debug.Log($"_defaultRadius = {_defaultRadius}");
        _viewTransform=_characterController.GetComponentInChildren<ViewTransform>().gameObject.transform;
    }
   
}
