using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject _mainPanel;
    [SerializeField] private GameObject _levelSelectPanel;

    private void Start()
    {
        // Гарантируем, что в главном меню мышь всегда видна и свободна
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // Метод для перехода к выбору уровней
    public void OpenLevelSelect()
    {
        _mainPanel.SetActive(false);
        _levelSelectPanel.SetActive(true);
    }
    public void ExitLevelSelect()
    {
        _mainPanel.SetActive(true);
        _levelSelectPanel.SetActive(false);
    }

    // Метод для возврата в главное меню
    public void CloseLevelSelect()
    {
        _levelSelectPanel.SetActive(false);
        _mainPanel.SetActive(true);
    }

    // Универсальный метод загрузки по имени (для кнопок)
    public void LoadLevelByName(string levelName)
    {
        Debug.Log($"Загрузка уровня: {levelName}");
        SceneLoader.LoadLevel(levelName);
    }

    public void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}