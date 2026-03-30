using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class PauseController : IInitializable, IDisposable
{
    private readonly GameManager _gameManager;
    private readonly Controls _controls;
    public PauseController(GameManager gameManager, Controls controls)
    {
        _gameManager = gameManager;
        _controls = controls;
    }

    public void Initialize()
    {
        _controls.Enable();
        _controls.Player.Escape.performed += OnEscapePerformed;
    }

    public void Dispose()
    {
        _controls.Player.Escape.performed -= OnEscapePerformed;
    }

    private void OnEscapePerformed(InputAction.CallbackContext context)
    {
        if (_gameManager.CurrentState == GameState.Playing)
            _gameManager.UpdateState(GameState.Paused);
        else if (_gameManager.CurrentState == GameState.Paused)
            _gameManager.UpdateState(GameState.Playing);
    }
}