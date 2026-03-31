using UnityEngine;
using Zenject;

public class ProjectInstaller : MonoInstaller
{
    [SerializeField] private LoadingView _loadingViewPrefab;
    [SerializeField] private RestartView _restartViewPrefab;
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
             .NonLazy(); // Создаем сразу при старте игры


        Container.Bind<RestartView>()
            .FromComponentInNewPrefab(_restartViewPrefab)
            .AsSingle()
            .OnInstantiated<RestartView>((ctx, view) => Object.DontDestroyOnLoad(view.transform.root.gameObject))
            .NonLazy();

        Container.Bind<SceneLoader>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<PauseController>().AsSingle().NonLazy();

        //// Биндим шину событий (если она еще не там)
        //Container.BindInterfacesAndSelfTo<LevelEventBus>().AsSingle();

        //// Биндим наш навигатор
        //Container.BindInterfacesAndSelfTo<LevelNavigationController>().AsSingle().NonLazy();

    }
}