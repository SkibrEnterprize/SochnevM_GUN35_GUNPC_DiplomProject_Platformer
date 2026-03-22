
using Player;
using UnityEngine;
using Zenject;

public class UiInstaller : MonoInstaller
{
    [SerializeField] private HealthView _healthView;
    [SerializeField] private ScoreItemView _itemView;
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<CollectItemEventBus>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<HealthPresenter>()
                .AsSingle()
                .WithArguments(_healthView)
                .NonLazy();

        Container.BindInterfacesAndSelfTo<CollectItemObserver>()
                .AsSingle()
                .WithArguments(_itemView)
                .NonLazy();
    }
}
