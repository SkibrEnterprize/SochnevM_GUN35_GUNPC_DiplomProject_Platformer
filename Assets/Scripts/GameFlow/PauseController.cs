using System;
using System.Threading.Tasks;
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
    private bool _isRestarting = false;
    private float _restartHoldTime = 0.5f; // Время удержания для рестарта

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

    public void Initialize()
    {
        _controls.Enable();

        // Пауза (Escape)
        _controls.Player.Escape.performed += OnEscapePerformed;

        // Таймскейл (Tab)
        _controls.Player.SlowMo.performed += OnTabPerformed;

        // Рестарт (~) - используем события нажатия и отпускания
        _controls.Player.Restart.started += OnRestartStarted;
        _controls.Player.Restart.canceled += OnRestartCanceled;
    }

    public void Dispose()
    {
        _controls.Player.Escape.performed -= OnEscapePerformed;
        _controls.Player.SlowMo.performed -= OnTabPerformed;
        _controls.Player.Restart.started -= OnRestartStarted;
        _controls.Player.Restart.canceled -= OnRestartCanceled;
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
        // Не даем менять таймскейл, если игра на системной паузе (меню)
        if (_gameManager.CurrentState != GameState.Playing) return;

        _isTimePaused = !_isTimePaused;
        Time.timeScale = _isTimePaused ? 0f : 1f;
        Debug.Log(_isTimePaused ? "Время остановлено" : "Время запущено");
    }

    public async void OnRestartStarted(InputAction.CallbackContext context)
    {
        _isRestarting = true;
        float elapsed = 0;

        _restartView.Show(); // Показываем ползунок

        while (_isRestarting && elapsed < _restartHoldTime)
        {
            elapsed += Time.unscaledDeltaTime;

            // Передаем процент заполнения (0.0 до 1.0)
            _restartView.UpdateProgress(elapsed / _restartHoldTime);

            await Task.Yield();
        }

        if (_isRestarting) // Удержали до конца
        {
            _isRestarting = false;
            _restartView.Hide();
            RestartLevel();
        }
    }

    public void OnRestartCanceled(InputAction.CallbackContext context)
    {
        _isRestarting = false;
        _restartView.Hide(); // Скрываем, если отпустили раньше времени
    }

    private void RestartLevel()
    {
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        _sceneLoader.LoadLevel(currentScene);
    }
}