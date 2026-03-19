using UnityEngine;
using Zenject;

public class AudioInstaller : MonoInstaller
{
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private SoundLibrary _soundLibrary;
    public override void InstallBindings()
    {
        Container.Bind<IAudioManager>()
                    .To<AudioManager>()
                    .FromInstance(_audioManager)
                    .AsSingle()
                    .NonLazy();       

        // –егистрируем библиотеку звуков
        Container.BindInstance(_soundLibrary)
            .AsSingle()
            .NonLazy();
        // ѕозвол€ем Zenject внедрить зависимости (IAudioService) внутрь ScriptableObject
        Container.QueueForInject(_soundLibrary);
    }
}

