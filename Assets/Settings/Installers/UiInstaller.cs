
using Player;
using UnityEngine;
using Zenject;

public class UiInstaller : MonoInstaller
{
    [SerializeField] private HealthView _healthView;
    [SerializeField] private ScoreItemView _ItemView;

    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<HealthPresenter>()
                .FromMethod(ctx =>
                {
                    var container = ctx.Container;
                    var healthModel = container.Resolve<HealthModel>();
                    return new HealthPresenter(
                        healthModel,
                        _healthView);

                })
                .AsSingle()
                .NonLazy();

        Container.BindInterfacesAndSelfTo<CollectItemPresenter>()
               .FromMethod(ctx =>
               {
                   var container = ctx.Container;
                   var ItemModel = container.Resolve<CollectItemModel>();
                   return new CollectItemPresenter(
                       ItemModel,
                       _ItemView);
               })
               .AsSingle()
               .NonLazy();
    }
}
