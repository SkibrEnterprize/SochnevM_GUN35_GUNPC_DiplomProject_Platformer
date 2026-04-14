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
        await _loadingView.FadeIn();
        _loadingView.UpdateProgress(0);

        float startTime = Time.realtimeSinceStartup;
        var operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        while (operation.progress < 0.9f)
        {
            _loadingView.UpdateProgress(operation.progress / 0.9f);
            await Task.Yield();
        }

        while (Time.realtimeSinceStartup - startTime < _minLoadingTime)
        {
            _loadingView.UpdateProgress(1f);
            await Task.Yield();
        }

        operation.allowSceneActivation = true;

        while (!operation.isDone)
            await Task.Yield();

        await _loadingView.FadeOut();
    }
}