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

            // 2. Биндим Управление
            var controls = new Controls();
            controls.Enable();                     // включаем все action‑maps
            Container.Bind<Controls>().FromInstance(controls).AsSingle();

            // Биндим конфиг игрока
            Container.Bind<PlayerConfig>().FromInstance(_playerConfig);

            // 3. Создаём MovementComponent 
            Container.BindInterfacesAndSelfTo<MovementComponent>()
                .FromMethod(ctx =>
                {
                    var container = ctx.Container;
                    var soundLibrary = container.Resolve<SoundLibrary>();
                    return new MovementComponent(
                        character,
                        controls,
                        _playerConfig,
                        soundLibrary);

                })
                .AsSingle()
                .NonLazy();

            // 7. Создаем CheckPointHandler
            Container.BindInterfacesAndSelfTo<CheckPointHolder>()
                .FromMethod(ctx =>
                {
                    var container = ctx.Container;
                    var soundLibrary = container.Resolve<SoundLibrary>();
                    return new CheckPointHolder(
                        _player.transform.position,
                        _player.transform.rotation,
                        soundLibrary);
                })
                .AsSingle()
                .NonLazy();

            // 4. Создаём HealthComponent
            Container.BindInterfacesAndSelfTo<HealthModel>()
                .FromMethod(ctx =>
                {
                    var container = ctx.Container;
                    var movementComponent = container.Resolve<MovementComponent>();
                    var soundLibrary = container.Resolve<SoundLibrary>();
                    return new HealthModel(
                        _playerConfig,
                        movementComponent,
                        soundLibrary);

                })
                .AsSingle()
                .NonLazy();

            // 7. Создаем PlayerDeathHandler
            Container.BindInterfacesAndSelfTo<PlayerDeathHandler>()                
                .FromMethod(ctx =>
                {
                    var checkPointHolder = ctx.Container.Resolve<CheckPointHolder>();
                    return new PlayerDeathHandler(
                    checkPointHolder,
                    _player.transform);
    })
    .AsSingle()
    .NonLazy();
            
            // 5. Создаем LevelFinishObserver
            Container.BindInterfacesAndSelfTo<LevelFinishSystem>()
                .FromMethod(ctx =>
                {
                    var container = ctx.Container;
                    return new LevelFinishSystem(
                        _levelFinishConfig);
                })
                .AsSingle()
                .NonLazy();

            // 6. Создаем CollectItemModel
            Container.BindInterfacesAndSelfTo<CollectItemModel>()
                .FromMethod(ctx =>
                {
                    var container = ctx.Container;
                    var soundLibrary = container.Resolve<SoundLibrary>();
                    return new CollectItemModel(
                        _levelFinishConfig,
                        soundLibrary);
                })
                .AsSingle()
                .NonLazy();


            Container.BindInterfacesAndSelfTo<PlayerEventObserver>()
                .FromMethod(ctx =>
                {
                    var container = ctx.Container;
                    var playerDeathHandler = container.Resolve<PlayerDeathHandler>();
                    var healthModel = container.Resolve<HealthModel>();
                    var movementComponent = container.Resolve<MovementComponent>();
                    return new PlayerEventObserver(playerDeathHandler,
                        healthModel,
                        movementComponent);

                })
                .AsSingle()
                .NonLazy();



        }
    }
}