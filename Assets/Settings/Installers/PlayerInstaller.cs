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

            // 2. Биндим Controls один раз (если нужен глобально)
            var controls = new Controls();
            controls.Enable();                     // включаем все action‑maps
            Container.Bind<Controls>().FromInstance(controls).AsSingle();

            // 3. Создаём JumpComponent через конструктор и сразу создаём его
            Container.BindInterfacesAndSelfTo<MovementComponent>()
                .FromMethod(ctx =>
                {
                    var container = ctx.Container;
                    return new MovementComponent(
                        character,
                        controls,
                        playerConfig);

                })
                .AsSingle()
                .NonLazy();
        }
    }
}