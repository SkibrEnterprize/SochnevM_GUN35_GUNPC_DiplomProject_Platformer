using UnityEngine;
using Zenject;

public class AudioInstaller : MonoInstaller
{
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private SoundLibrary _soundLibrary;

    public override void InstallBindings()
    {
        // 1. Биндим шину событий как синглтон для этого контекста
        Container.BindInterfacesAndSelfTo<SoundEventBus>().AsSingle().NonLazy();

        // 2. Биндим саму библиотеку (ScriptableObject)
        Container.BindInstance(_soundLibrary).AsSingle().NonLazy();

        // 3. Биндим AudioManager (уже существующий на сцене)
        Container.Bind<IAudioManager>().FromInstance(_audioManager).AsSingle().NonLazy();

        // 4. Биндим Observer, чтобы он создался и подписался на шину
        // Используем BindInterfacesTo, чтобы Zenject вызвал Initialize()
        Container.BindInterfacesTo<SoundEffectObserver>().AsSingle().NonLazy();
    }
    //public override void InstallBindings()
    //{
    //    Container.Bind<IAudioManager>()
    //                .To<AudioManager>()
    //                .FromInstance(_audioManager)
    //                .AsSingle()
    //                .NonLazy();       

    //    // Регистрируем библиотеку звуков
    //    Container.BindInstance(_soundLibrary)
    //        .AsSingle()
    //        .NonLazy();
    //    // Позволяем Zenject внедрить зависимости (IAudioService) внутрь ScriptableObject
    //    Container.QueueForInject(_soundLibrary);
    //}
}

