using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PlayerStartParameters
{
    private CharacterController _characterController;
    private float _defaultHeight;
    private float _defaultRadius;
    private Transform _viewTransform;

    private Transform _combatVFXPoint;

    public float DefaultHeight => _defaultHeight;
    public float DefaultRadius => _defaultRadius;
    public Transform ViewTransform => _viewTransform; 
    public Transform CombatVFXPoint => _combatVFXPoint;
    public PlayerStartParameters(CharacterController controller)
    {
        _characterController = controller;
        _defaultHeight = _characterController.height;
        _defaultRadius = _characterController.radius;
        Debug.Log($"_defaultHeight = {_defaultHeight}");
        Debug.Log($"_defaultRadius = {_defaultRadius}");
        _viewTransform=_characterController.GetComponentInChildren<ViewTransform>().transform;
        _combatVFXPoint = _characterController.GetComponentInChildren<CombatVFXPoint>().transform;
    }
   
}
