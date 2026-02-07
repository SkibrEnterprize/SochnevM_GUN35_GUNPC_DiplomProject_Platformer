using Player;
using UnityEngine;
using Zenject;
using Zenject.SpaceFighter;

public class SceneInstaller : MonoInstaller
{
    private Controls _controls;
    public override void InstallBindings()
    {
        Debug.Log("Controls is Bind and activate!!!");
        _controls = new Controls();
        _controls.Enable();
        Container.Bind<Controls>()
            .FromInstance(_controls)
            .AsSingle()
            .NonLazy();
    }
}

