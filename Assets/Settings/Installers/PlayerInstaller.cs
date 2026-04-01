using UnityEngine;
using Zenject;

namespace Player
{
    public sealed class PlayerInstaller : MonoInstaller
    {
        [Header("Scene Objects")]
        [SerializeField] private GameObject _player;
        [SerializeField] private Animator _playerAnimator;

        [Header("Configs")]
        [SerializeField] private PlayerConfig _playerConfig;
        [SerializeField] private CombatConfig _combatConfig;

        public override void InstallBindings()
        {
            // 1. Извлекаем компоненты Unity
            var character = _player.GetComponent<CharacterController>();

            // 2. Биндим компоненты как инстансы (теперь они доступны везде в сцене)
            Container.Bind<CharacterController>().FromInstance(character).AsSingle();
            Container.Bind<Animator>().FromInstance(_playerAnimator).AsSingle();

            // 3. Биндим конфиги
            Container.Bind<PlayerConfig>().FromInstance(_playerConfig).AsSingle();
            Container.Bind<CombatConfig>().FromInstance(_combatConfig).AsSingle();

            // 4. Биндим системы логики (Zenject сам подставит Animator и CharacterController в конструкторы)

            Container.BindInterfacesAndSelfTo<PlayerAnimator>()
                .AsSingle()
                .NonLazy();

            Container.BindInterfacesAndSelfTo<PlayerMovementSystem>()
                .AsSingle()
                .NonLazy();

            Container.BindInterfacesAndSelfTo<CombatComponent>()
                .AsSingle()
                .NonLazy();

            Container.BindInterfacesAndSelfTo<HealthModel>()
                .AsSingle()
                .NonLazy();

            // 5. Биндим остальные вспомогательные системы
            Container.BindInterfacesAndSelfTo<PlayerStartParameters>()
                .AsSingle()
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
        //    [SerializeField] private GameObject _player;
        //    [SerializeField] private Animator _playerAnimator;
        //    [SerializeField] private PlayerConfig _playerConfig;
        //    [SerializeField] private CombatConfig _combatConfig;

        //    public override void InstallBindings()
        //    {
        //        var character = _player.GetComponent<CharacterController>();

        //        Container.Bind<PlayerConfig>().FromInstance(_playerConfig);

        //        Container.BindInterfacesAndSelfTo<PlayerStartParameters>()
        //            .AsSingle()
        //            .WithArguments(character)
        //            .NonLazy();

        //        Container.BindInterfacesAndSelfTo<PlayerMovementSystem>()
        //            .AsSingle()
        //            .WithArguments(character, _playerConfig)
        //            .NonLazy();

        //        Container.BindInterfacesAndSelfTo<CombatComponent>()
        //            .AsSingle()
        //            .WithArguments(_combatConfig, character)
        //            .NonLazy();

        //        Container.BindInterfacesAndSelfTo<HealthModel>()
        //            .AsSingle()
        //            .WithArguments(_playerConfig)
        //            .NonLazy();

        //        Container.BindInterfacesAndSelfTo<PlayerDeathObserver>()
        //            .AsSingle()
        //            .NonLazy();

        //        Container.BindInterfacesAndSelfTo<CollectItemModel>()
        //            .AsSingle()
        //            .NonLazy();

        //        Container.BindInterfacesAndSelfTo<PlayerEventObserver>()
        //            .AsSingle()
        //            .NonLazy();
        //        Container
        //        .BindInterfacesAndSelfTo<PlayerAnimator>()
        //        .AsSingle()
        //        .WithArguments(_playerAnimator, character)
        //        .NonLazy();

        //    }
        //}
    }