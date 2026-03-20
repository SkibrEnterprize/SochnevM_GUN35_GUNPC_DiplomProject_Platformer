using Player;
using System;
using Zenject;

public class SoundEffectObserver: IInitializable, IDisposable
{
    private SoundLibrary _soundLibrary;
    private HealthModel _healthModel;
    private PlayerDeathHandler _playerDeathHandler;
    private MovementComponent _movementComponent;
    private readonly LevelFinishSystem _levelFinishSystem;


    public SoundEffectObserver(SoundLibrary soundLibrary,
                                HealthModel healthModel,
                                PlayerDeathHandler playerDeathHandler,
                                MovementComponent movementComponent,
                                LevelFinishSystem levelFinishSystem)
    {
        _soundLibrary = soundLibrary;
        _healthModel = healthModel;
        _playerDeathHandler = playerDeathHandler;
        _movementComponent = movementComponent;
        _levelFinishSystem = levelFinishSystem;
    }
    public void Initialize()
    {
        _healthModel.OnHealthIsDown += PlayHealthSound;
        _playerDeathHandler.OnPlayerDied += PlayDieSound;
        _movementComponent.OnJump += PlayJumpSound;
        _movementComponent.OnSideJump += PlaySideJumpSound;
        _levelFinishSystem.OnEndPointReached += PlayEndPointReachedSound;
        _levelFinishSystem.OnLevelFinished += PlayLevelFinishSound;
    }

    private void PlayDieSound()
    {
        _soundLibrary.RequestPlay(SoundType.Die);
    }

    private void PlaySideJumpSound()
    {
        _soundLibrary.RequestPlay(SoundType.WallJump);
    }

    private void PlayJumpSound()
    {
        _soundLibrary.RequestPlay(SoundType.Jump);
    }

    private void PlayHealthSound(bool obj)
    {
        _soundLibrary.RequestPlay(obj ? SoundType.Healing : SoundType.Damage);
    }

    private void PlayLevelFinishSound()
    {
        _soundLibrary.RequestPlay(SoundType.Finish);
    }

    private void PlayEndPointReachedSound()
    {
        _soundLibrary?.RequestPlay(SoundType.EndPoint);
    }

    public void Dispose()
    {
        _healthModel.OnHealthIsDown -= PlayHealthSound;
        _playerDeathHandler.OnPlayerDied -= PlayDieSound;
        _movementComponent.OnJump -= PlayJumpSound;
        _movementComponent.OnSideJump -= PlaySideJumpSound;
        _levelFinishSystem.OnEndPointReached -= PlayEndPointReachedSound;
        _levelFinishSystem.OnLevelFinished -= PlayLevelFinishSound;
    }

}
