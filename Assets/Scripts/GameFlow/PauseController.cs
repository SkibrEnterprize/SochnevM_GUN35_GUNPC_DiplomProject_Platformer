using System;
using System.Collections;
using UnityEngine.InputSystem.Interactions;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class PauseController : IInitializable, IDisposable
{
    private readonly GameManager _gameManager;
    private readonly SceneLoader _sceneLoader;
    private readonly Controls _controls;
    private readonly RestartView _restartView;

    private bool _isTimePaused = false;
    public PauseController(GameManager gameManager, 
        SceneLoader sceneLoader, 
        Controls controls, 
        RestartView restartView)
    {
        _gameManager = gameManager;
        _sceneLoader = sceneLoader;
        _controls = controls;
        _restartView = restartView;
    }

    private Coroutine _restartCoroutine;

    public void Initialize()
    {
        _controls.Enable();
        _controls.Player.Escape.performed += OnEscapePerformed;
        _controls.Player.SlowMo.performed += OnTabPerformed;

        _controls.Player.Restart.started += OnRestartStarted; 
        _controls.Player.Restart.performed += OnRestartPerformed; 
        _controls.Player.Restart.canceled += OnRestartCanceled; 
    }

    public void Dispose()
    {
        _controls.Player.Escape.performed -= OnEscapePerformed;
        _controls.Player.SlowMo.performed -= OnTabPerformed;
        _controls.Player.Restart.started -= OnRestartStarted;
        _controls.Player.Restart.performed -= OnRestartPerformed;
        _controls.Player.Restart.canceled -= OnRestartCanceled;
    }

    private void OnRestartStarted(InputAction.CallbackContext context)
    {
        _restartView.Show();
               
        float holdDuration = 0.5f; 

        if (context.interaction is HoldInteraction hold)
        {
            holdDuration = hold.duration;
        }

        if (_restartCoroutine != null) _restartView.StopCoroutine(_restartCoroutine);
        _restartCoroutine = _restartView.StartCoroutine(UpdateRestartProgress(holdDuration));
    }

    private void OnRestartPerformed(InputAction.CallbackContext context)
    {
        StopRestartVisual();
        RestartLevel();
    }

    private void OnRestartCanceled(InputAction.CallbackContext context)
    {
        StopRestartVisual();
    }

    private void StopRestartVisual()
    {
        if (_restartCoroutine != null) _restartView.StopCoroutine(_restartCoroutine);
        _restartView.Hide();
    }

    private IEnumerator UpdateRestartProgress(float duration)
    {
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            _restartView.UpdateProgress(elapsed / duration);
            yield return null;
        }
    }
    private void OnEscapePerformed(InputAction.CallbackContext context)
    {
        if (_gameManager.CurrentState == GameState.Playing)
            _gameManager.UpdateState(GameState.Paused);
        else if (_gameManager.CurrentState == GameState.Paused)
            _gameManager.UpdateState(GameState.Playing);
    }

    private void OnTabPerformed(InputAction.CallbackContext context)
    {
        if (_gameManager.CurrentState != GameState.Playing) return;

        _isTimePaused = !_isTimePaused;
        Time.timeScale = _isTimePaused ? 0f : 1f;
        Debug.Log(_isTimePaused ? "Время остановлено" : "Время запущено");
    }
    private void RestartLevel()
    {
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        _sceneLoader.LoadLevel(currentScene);
    }
}