using Zenject;

public class CheckPointSystemInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<CheckPointModel>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<CheckPointEventBus>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<CheckPointObserver>().AsSingle().NonLazy();       
    }
}
