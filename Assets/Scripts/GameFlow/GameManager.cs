using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class GameManager:  IInitializable
{
    public event Action<GameState> OnStateChanged;
    public GameState CurrentState { get; private set; }
    public void Initialize()
    {
        // Проверяем, какая сцена сейчас активна
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "MainMenu")
        {
            UpdateState(GameState.MainMenu);
        }
        else
        {            
            UpdateState(GameState.Playing);
        }
    }
    public void UpdateState(GameState newState)
    {
        CurrentState = newState;

        switch (newState)
        {
            case GameState.MainMenu:
                Time.timeScale = 1f;
                Cursor.visible = true;             // Показываем курсор
                Cursor.lockState = CursorLockMode.None; // Разблокируем его
                break;
            case GameState.Playing:
                Time.timeScale = 1f;
                Cursor.visible = false;
                break;
            case GameState.Paused:
                Time.timeScale = 0f; // Останавливаем всё движение
                Cursor.visible = true;
                break;
            case GameState.GameOver:
                Time.timeScale = 0.5f; // Слоу-мо при смерти
                Cursor.visible = true;
                break;
        }

        OnStateChanged?.Invoke(newState);
    }

}
