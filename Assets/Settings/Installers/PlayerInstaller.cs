using UnityEngine;
using Zenject;

namespace Player
{
    public sealed class PlayerInstaller : MonoInstaller
    {
        [Header("Scene Objects")]
        [SerializeField] private GameObject _player;
        [SerializeField] private Animator _playerAnimator;
        [SerializeField] private SkinnedMeshRenderer _skinnedMeshRenderer;

        [Header("Configs")]
        [SerializeField] private PlayerConfig _playerConfig;
        [SerializeField] private CombatConfig _combatConfig;
        [SerializeField] private CombatUI _combatUi;

        public override void InstallBindings()
        {
            var character = _player.GetComponent<CharacterController>();

            Container.Bind<CharacterController>().FromInstance(character).AsSingle();
            Container.Bind<Animator>().FromInstance(_playerAnimator).AsSingle();
            Container.Bind<SkinnedMeshRenderer>().FromInstance(_skinnedMeshRenderer).AsSingle();

            Container.Bind<PlayerConfig>().FromInstance(_playerConfig).AsSingle();
            Container.Bind<CombatConfig>().FromInstance(_combatConfig).AsSingle();

            Container.BindInterfacesAndSelfTo<PlayerAnimator>()
                .AsSingle()
                .NonLazy();

            Container.BindInterfacesAndSelfTo<PlayerMovementSystem>()
                .AsSingle()
                .NonLazy();

            Container.BindInterfacesAndSelfTo<CombatComponent>()
                .AsSingle()
                .WithArguments(_combatUi)
                .NonLazy();

            Container.BindInterfacesAndSelfTo<HealthModel>()
                .AsSingle()
                .NonLazy();

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
}