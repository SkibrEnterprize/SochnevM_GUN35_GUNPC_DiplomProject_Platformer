using UnityEngine;
using Zenject;

namespace Player
{
    public sealed class PlayerInstaller : MonoInstaller
    {
        [SerializeField] private GameObject _player;
        [SerializeField] private PlayerConfig _playerConfig;
        [SerializeField] private CombatConfig _combatConfig;

        public override void InstallBindings()
        {
            var character = _player.GetComponent<CharacterController>();

            //// 2. Биндим Управление
            //var controls = new Controls();
            //controls.Enable();                     // включаем все action‑maps
            //Container.Bind<Controls>().FromInstance(controls).AsSingle();

            Container.Bind<PlayerConfig>().FromInstance(_playerConfig);

            Container.BindInterfacesAndSelfTo<PlayerStartParameters>()
                .AsSingle()
                .WithArguments(character)
                .NonLazy();

            Container.BindInterfacesAndSelfTo<PlayerMovementSystem>()
                .AsSingle()
                .WithArguments(character, _playerConfig)
                .NonLazy();

            Container.BindInterfacesAndSelfTo<CombatComponent>()
                .AsSingle()
                .WithArguments(_combatConfig, character)
                .NonLazy();

            Container.BindInterfacesAndSelfTo<HealthModel>()
                .AsSingle()
                .WithArguments(_playerConfig)
                .NonLazy();

            Container.BindInterfacesAndSelfTo<PlayerDeathObserver>()
                .AsSingle()
                .NonLazy();

            Container.BindInterfacesAndSelfTo<CollectItemModel>()
                .AsSingle()
                .NonLazy();

            Container.BindInterfacesAndSelfTo<PlayerEventObserver>()
                .AsSingle()
                .NonLazy();

        }
    }
}