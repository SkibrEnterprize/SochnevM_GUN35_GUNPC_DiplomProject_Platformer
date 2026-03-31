using Zenject;

public class GameFlowInstaller : MonoInstaller
{
    public override void InstallBindings()
    {        
        var controls = new Controls();
        controls.Enable();
        //Container.Bind<Controls>().FromInstance(controls).AsSingle();

        Container.BindInterfacesAndSelfTo<GameManager>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<PauseController>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<SceneLoader>().AsSingle().NonLazy();
    }
}
