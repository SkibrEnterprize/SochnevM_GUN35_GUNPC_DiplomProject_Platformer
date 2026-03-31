using System;
using Zenject;
using UnityEngine;

public class LevelNavigationController : IInitializable, IDisposable
{
    private readonly ILevelEventBus _levelBus;
    private readonly SceneLoader _sceneLoader;
    private readonly LevelFinishConfig _config;

    public LevelNavigationController(ILevelEventBus levelBus, SceneLoader sceneLoader, LevelFinishConfig config)
    {
        _levelBus = levelBus;
        _sceneLoader = sceneLoader;
        _config = config;
    }

    public void Initialize() => _levelBus.OnLevelFinished += HandleFinish;
    public void Dispose() => _levelBus.OnLevelFinished -= HandleFinish;

    private void HandleFinish()
    {
        // Проверяем, вписано ли имя следующей сцены в конфиг
        if (!string.IsNullOrEmpty(_config.NextSceneName))
        {
            _sceneLoader.LoadLevel(_config.NextSceneName);
        }
    }
}