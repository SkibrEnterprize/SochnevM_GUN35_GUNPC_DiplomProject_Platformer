using UnityEngine;
using VFX;
using Zenject;

public class VFXInstaller : MonoInstaller
{
    [SerializeField] private VFXSystem _vfxSystem;
    [SerializeField] private VFXLibrary _vfxLibrary;

    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<VFXEventBus>().AsSingle().NonLazy();
        Container.BindInstance(_vfxLibrary).AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<VFXSystem>().FromInstance(_vfxSystem).AsSingle().NonLazy();
        Container.BindInterfacesTo<VFXObserver>().AsSingle().NonLazy();
    }
}
