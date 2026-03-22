using Zenject;

public class HealthSystemInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<HealthEventBus>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<HealthChangeObserver>().AsSingle().NonLazy();
    }
}
