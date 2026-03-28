using UnityEngine;
using Zenject;

public class PlayerStartParameters
{
    private CharacterController _characterController;
    private float _defaultHeight;
    private float _defaultRadius;

    public float DefaultHeight => _defaultHeight;
    public float DefaultRadius => _defaultRadius;
    public PlayerStartParameters(CharacterController controller)
    {
        _characterController = controller;
        _defaultHeight = _characterController.height;
        _defaultRadius = _characterController.radius;
        Debug.Log($"_defaultHeight = {_defaultHeight}");
        Debug.Log($"_defaultRadius = {_defaultRadius}");
    }
   
}
