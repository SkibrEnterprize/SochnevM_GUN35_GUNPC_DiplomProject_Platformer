using UnityEngine;
using Zenject;

namespace Player
{
    public sealed class PlayerInstaller : MonoInstaller
    {
        [SerializeField] private GameObject _player;
        [SerializeField] private PlayerConfig _playerConfig;

        public override void InstallBindings()
        {
            var character = _player.GetComponent<CharacterController>();
            var playerConfig = _playerConfig;

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
                    var playerConfig = container.Resolve<PlayerConfig>();
                    return new MovementComponent(
                        character,
                        controls,
                        playerConfig,
                        signalBus);

                })
                .AsSingle()
                .NonLazy();

            // 4. Создаём HealthComponent
            Container.BindInterfacesAndSelfTo<HealthComponent>()
                .FromMethod(ctx =>
                {
                    var container = ctx.Container;
                    var signalBus = container.Resolve<SignalBus>();
                    var playerConfig = container.Resolve<PlayerConfig>();
                    return new HealthComponent(                       
                        playerConfig,
                        signalBus);

                })
                .AsSingle()
                .NonLazy();
                       
        }
    }
}