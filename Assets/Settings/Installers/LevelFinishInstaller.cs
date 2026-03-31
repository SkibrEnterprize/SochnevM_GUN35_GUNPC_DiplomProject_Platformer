using UnityEngine;
using Zenject;

public class LevelFinishInstaller : MonoInstaller
{
    [SerializeField] private LevelFinishConfig _levelFinishConfig;
    public override void InstallBindings()
    {
        Container.BindInstance(_levelFinishConfig).AsSingle();
        Container.BindInterfacesAndSelfTo<LevelEventBus>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<LevelFinishSystem>()
            .AsSingle()
            .WithArguments(_levelFinishConfig)
            .NonLazy();
        Container.BindInterfacesAndSelfTo<LevelFinishObserver>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<LevelPointObserver>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<LevelNavigationController>().AsSingle().NonLazy();

    }
}

