using Zenject;

public class SceneInstaller : MonoInstaller
{
    public override void InstallBindings()
    {     
        SignalBusInstaller.Install(Container);
        Container.DeclareSignal<Player.Signals.FallDistanceSignal>();
        Container.DeclareSignal<Player.Signals.LevelFinishCollectSignal>();
    }
}

