using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader
{
    private readonly LoadingView _loadingView;
    private readonly float _minLoadingTime = 1.5f;

    public SceneLoader(LoadingView loadingView)
    {
        _loadingView = loadingView;
    }

    public async void LoadLevel(string sceneName)
    {
        // 1. Плавно проявляем экран загрузки
        await _loadingView.FadeIn();
        _loadingView.UpdateProgress(0);

        float startTime = Time.realtimeSinceStartup;
        var operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        // 2. Идем до 90% (загрузка данных)
        while (operation.progress < 0.9f)
        {
            _loadingView.UpdateProgress(operation.progress / 0.9f);
            await Task.Yield();
        }

        // 3. Искусственно тянем время, если загрузка была слишком быстрой
        // Например, гарантируем минимум 1.5 секунды на экране
        while (Time.realtimeSinceStartup - startTime < _minLoadingTime)
        {
            // Дотягиваем прогресс до 100% за это время
            _loadingView.UpdateProgress(1f);
            await Task.Yield();
        }

        // 4. Активируем сцену
        operation.allowSceneActivation = true;

        while (!operation.isDone)
            await Task.Yield();

        // 5. Плавно скрываем
        await _loadingView.FadeOut();
        //_loadingView.Show();
        //_loadingView.UpdateProgress(0);

        //var operation = SceneManager.LoadSceneAsync(sceneName);
        //operation.allowSceneActivation = false;

        //// Эмулируем плавный прогресс до 90% (особенности Unity)
        //while (operation.progress < 0.9f)
        //{
        //    _loadingView.UpdateProgress(operation.progress / 0.9f);
        //    await Task.Yield();
        //}

        //// Завершаем полоску до 100%
        //_loadingView.UpdateProgress(1f);

        //// Небольшая задержка, чтобы игрок успел увидеть 100%
        //await Task.Delay(500);

        //operation.allowSceneActivation = true;

        //while (!operation.isDone)
        //{
        //    await Task.Yield();
        //}

        //_loadingView.Hide();
    }
}