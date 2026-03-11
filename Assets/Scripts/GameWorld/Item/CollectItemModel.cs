using Player.Signals;
using System;
using UnityEngine;
using Zenject;

public class CollectItemModel : IInitializable, IDisposable, ICollectItemObserver
{
    private readonly SignalBus _signalBus;
    private int _score;

    public event Action<int> OnCountChanged;
    public int Score
    {
        get => _score;
        set
        {
           {
                _score++;
                OnCountChanged?.Invoke(_score);
                Debug.Log("Health Update ON Envoke");
            }
        }
    }
    public CollectItemModel(SignalBus signalBus,
        LevelFinishConfig levelFinishConfig)
    {
        _signalBus = signalBus;
    }

    public void Initialize()
    {
        _signalBus.Subscribe<CollectItemSignal>(OnCollectItem);
    }
    
    public void Dispose()
    {
        _signalBus.Unsubscribe<CollectItemSignal>(OnCollectItem);
    }


    public void OnCollectItem()
    {
        Score++;
        Debug.Log($"Collect Item = {_score}");
    }
    
}
