using Player.Signals;
using System;
using UnityEngine;
using Zenject;

public class CollectItemObserver : IInitializable, IDisposable, ICollectItemObserver
{
    private readonly SignalBus _signalBus;
    private int _count;


    public CollectItemObserver(SignalBus signalBus,
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
        _count++;
        Debug.Log($"Collect Item = {_count}");
    }
    
}
