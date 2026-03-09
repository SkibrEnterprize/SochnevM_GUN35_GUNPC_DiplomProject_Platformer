using UnityEngine;
using Zenject;

namespace Player
{
    public sealed class PlayerInstaller : MonoInstaller
    {
        [SerializeField] private GameObject _player;
        [SerializeField] private PlayerConfig _playerConfig;
        [SerializeField] private LevelFinishConfig _levelFinishConfig;

        public override void InstallBindings()
        {
            var character = _player.GetComponent<CharacterController>();
            //var playerConfig = _playerConfig;

            // 2. Биндим Управление
            var controls = new Controls();
            controls.Enable();                     // включаем все action‑maps
            Container.Bind<Controls>().FromInstance(controls).AsSingle();

            // Биндим конфиг игрока
            Container.Bind<PlayerConfig>().FromInstance(_playerConfig);

            // 3. Создаём MovementComponent через конструктор и сразу создаём его
            Container.BindInterfacesAndSelfTo<MovementComponent>()
                .FromMethod(ctx =>
                {
                    var container = ctx.Container;
                    var signalBus = container.Resolve<SignalBus>();
                    //var playerConfig = container.Resolve<PlayerConfig>();
                    return new MovementComponent(
                        character,
                        controls,
                        _playerConfig,
                        signalBus);

                })
                .AsSingle()
                .NonLazy();

            // 4. Создаём HealthComponent
            Container.BindInterfacesAndSelfTo<HealthModel>()
                .FromMethod(ctx =>
                {
                    var container = ctx.Container;
                    var signalBus = container.Resolve<SignalBus>();
                    //var playerConfig = container.Resolve<PlayerConfig>();
                    return new HealthModel(
                        _playerConfig,
                        signalBus);

                })
                .AsSingle()
                .NonLazy();

            // 5. Создаем LevelFinishObserver
            Container.BindInterfacesAndSelfTo<LevelFinishObserver>()
                .FromMethod(ctx =>
                {
                    var container = ctx.Container;
                    var signalBus = container.Resolve<SignalBus>();
                    //var levelFinishConfig = container.Resolve<LevelFinishConfig>();
                    return new LevelFinishObserver(
                        signalBus,
                        _levelFinishConfig);
                })
                .AsSingle()
                .NonLazy();

            // 6. Создаем CollectItemObserver
            Container.BindInterfacesAndSelfTo<CollectItemObserver>()
                .FromMethod(ctx =>
                {
                    var container = ctx.Container;
                    var signalBus = container.Resolve<SignalBus>();
                    return new CollectItemObserver(
                        signalBus,
                        _levelFinishConfig);
                })
                .AsSingle()
                .NonLazy();

            // 7. Создаем CheckPointHandler
            Container.BindInterfacesAndSelfTo<CheckPointHolder>()
                .FromMethod(ctx =>
                {
                    return new CheckPointHolder(
                        _player.transform.position,
                        _player.transform.rotation);
                })
                .AsSingle()
                .NonLazy();

            // 7. Создаем PlayerDeathHandler
            Container.BindInterfacesAndSelfTo<PlayerDeathHandler>()
                .FromMethod(ctx =>
                {
                    var container = ctx.Container;
                    var checkPointHolder = container.Resolve<CheckPointHolder>();
                    var signalBus = container.Resolve<SignalBus>();
                    return new PlayerDeathHandler(
                        checkPointHolder,
                        _player.transform,
                        signalBus);
                })
                .AsSingle()
                .NonLazy();
        }
    }
}