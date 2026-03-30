using UnityEngine;
using Zenject;

public class PauseMenuView : MonoBehaviour
{
    [SerializeField] private GameObject _pausePanel; // Твоя панелька с кнопками
    private GameManager _gameManager;

    [Inject]
    public void Construct(GameManager gameManager)
    {
        _gameManager = gameManager;
    }
    private void OnEnable() => _gameManager.OnStateChanged += HandleStateChange;
    private void OnDisable() => _gameManager.OnStateChanged -= HandleStateChange;

    private void HandleStateChange(GameState state)
    {
        // Показываем панель только если игра на паузе
        _pausePanel.SetActive(state == GameState.Paused);

        // Если пауза — включаем курсор, если игра — выключаем
        Cursor.lockState = (state == GameState.Paused) ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = (state == GameState.Paused);
    }

    // Метод для кнопки "Resume" (Продолжить) в UI
    public void OnResumeClicked()
    {
        // Находим GameManager через Zenject или синглтон и переключаем стейт
        // Здесь можно просто найти его на сцене для теста:
        _gameManager.UpdateState(GameState.Playing);
    }
    public void OnExitToMenuClicked()
    {
        // 1. Обязательно возвращаем время в норму, иначе в меню всё "замрет"
        _gameManager.UpdateState(GameState.MainMenu);

        // 2. Загружаем сцену меню через твой SceneLoader
        // Убедись, что сцена называется именно так, как в Build Settings
        SceneLoader.LoadLevel("MainMenu");
    }
}