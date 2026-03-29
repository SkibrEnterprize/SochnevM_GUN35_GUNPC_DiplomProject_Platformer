using UnityEngine;
using Zenject;

public class AudioInstaller : MonoInstaller
{
    [SerializeField] private AudioSystem _audioSystem;
    [SerializeField] private SoundLibrary _soundLibrary;

    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<SoundEventBus>().AsSingle().NonLazy();
        Container.BindInstance(_soundLibrary).AsSingle().NonLazy();
        Container.Bind<IAudioSystem>().FromInstance(_audioSystem).AsSingle().NonLazy();
        Container.BindInterfacesTo<SoundEffectObserver>().AsSingle().NonLazy();
    }
}

