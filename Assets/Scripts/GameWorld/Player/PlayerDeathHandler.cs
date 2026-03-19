
using Player.Signals;
using System;
using Zenject;
using UnityEngine;
using Player;


public class PlayerDeathHandler : IInitializable, IDisposable, IPlayerDeathHandler
{
    public event Action OnHealthRepair;

    [SerializeField] private Transform _playerTransform;
    private CheckPointHolder _checkPointHolder;
    

    public PlayerDeathHandler(CheckPointHolder chechPointHolder, Transform playerTransform)
    {
        _checkPointHolder = chechPointHolder;
        _playerTransform = playerTransform;        
    }
    public void Initialize()
    {
    }

    public void Dispose()
    {
    }


    public void PlayerDied()
    {
        Debug.Log("Plaer Died activate");
        MoveToLastCheckPoint();
        OnHealthRepair?.Invoke();
    }

    private void MoveToLastCheckPoint()
    {
        Vector3 pos = _checkPointHolder.GetPosition();
        Quaternion rot = _checkPointHolder.GetRotation();

        if (pos != default && rot != default)
        {
            _playerTransform.position = pos;
            _playerTransform.rotation = rot;
        }
    }
}
