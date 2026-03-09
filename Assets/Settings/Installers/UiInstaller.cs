
using Player;
using UnityEngine;
using Zenject;

public class UiInstaller : MonoInstaller
{
    [SerializeField] private HealthView _healthView;

    public override void InstallBindings()
    {                
        Container.BindInterfacesAndSelfTo<HealthPresenter>()
                .FromMethod(ctx =>
                {
                    var container = ctx.Container;
                    var healthModel = container.Resolve<HealthModel>();
                    //var playerConfig = container.Resolve<PlayerConfig>();
                    return new HealthPresenter(
                        healthModel,
                        _healthView);

                })
                .AsSingle()
                .NonLazy();        
    }
}
