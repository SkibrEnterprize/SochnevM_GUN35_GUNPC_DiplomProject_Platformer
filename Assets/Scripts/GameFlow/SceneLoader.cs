using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public static class SceneLoader
{
    public static async void LoadLevel(string sceneName)
    {
        var operation = SceneManager.LoadSceneAsync(sceneName);
        while (!operation.isDone)
        {
            // Тут можно обновлять полоску загрузки (UI)
            await Task.Yield();
        }
    }
}