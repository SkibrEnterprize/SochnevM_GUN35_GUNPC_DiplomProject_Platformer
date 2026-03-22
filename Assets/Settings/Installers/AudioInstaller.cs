using UnityEngine;
using Zenject;

public class AudioInstaller : MonoInstaller
{
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private SoundLibrary _soundLibrary;

    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<SoundEventBus>().AsSingle().NonLazy();
        Container.BindInstance(_soundLibrary).AsSingle().NonLazy();
        Container.Bind<IAudioManager>().FromInstance(_audioManager).AsSingle().NonLazy();
        Container.BindInterfacesTo<SoundEffectObserver>().AsSingle().NonLazy();
    }
}

