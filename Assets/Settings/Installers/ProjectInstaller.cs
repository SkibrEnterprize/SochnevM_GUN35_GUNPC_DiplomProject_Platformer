using UnityEngine;
using Zenject;

public class ProjectInstaller : MonoInstaller
{
    [SerializeField] private LoadingView _loadingViewPrefab;
    [SerializeField] private RestartView _restartViewPrefab;
    [SerializeField] private GameObject _musicManagerPrefab;
    [SerializeField] private MusicConfig _musicConfig;
    [SerializeField] private GameObject _settingsManagerPrefab;

    public override void InstallBindings()
    {        
        Container.BindInterfacesAndSelfTo<GameManager>().AsSingle().NonLazy();
        Container.Bind<Controls>().AsSingle().NonLazy();

        Container
             .Bind<LoadingView>()
             .FromComponentInNewPrefab(_loadingViewPrefab)
             .AsSingle()
             .OnInstantiated<LoadingView>((ctx, view) =>
             {
                 Object.DontDestroyOnLoad(view.transform.root.gameObject);
                 view.InitialHide();
             })
             .NonLazy();

        Container.Bind<RestartView>()
            .FromComponentInNewPrefab(_restartViewPrefab)
            .AsSingle()
            .OnInstantiated<RestartView>((ctx, view) => Object.DontDestroyOnLoad(view.transform.root.gameObject))
            .NonLazy();

        Container.Bind<SceneLoader>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<PauseController>().AsSingle().NonLazy();

        Container
            .Bind<MusicManager>()
            .FromComponentInNewPrefab(_musicManagerPrefab)
            .AsSingle()
            .NonLazy();

        Container
           .Bind<SettingsManager>()
           .FromComponentInNewPrefab(_settingsManagerPrefab)
           .AsSingle()
           .NonLazy();
    }
}