using UnityEngine;
using Zenject;

namespace Player
{
    public sealed class PlayerInstaller : MonoInstaller
    {
        [SerializeField] private GameObject _player;
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private PlayerConfig _playerConfig;

        public override void InstallBindings()
        {
            var rigidbody = _player.GetComponent<Rigidbody>();
            var feetPos = _player.transform.Find("FeetPosition");   // или любой другой Transform
            var groundLayer = _groundLayer;
            var playerConfig = _playerConfig;

            // 2. Биндим Controls один раз (если нужен глобально)
            var controls = new Controls();
            controls.Enable();                     // включаем все action‑maps
            Container.Bind<Controls>().FromInstance(controls).AsSingle();

            // 3. Создаём JumpComponent через конструктор и сразу создаём его
            Container.BindInterfacesAndSelfTo<JumpComponent>()
                .FromMethod(ctx =>
                {
                    var container = ctx.Container;
                    return new JumpComponent(
                        rigidbody,
                        feetPos,
                        groundLayer,
                        controls,
                        playerConfig);

                })
                .AsSingle()
                .NonLazy();
        }
    }
}