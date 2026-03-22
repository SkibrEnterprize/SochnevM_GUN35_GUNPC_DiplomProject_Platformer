using System;
using UnityEngine;
using Zenject;

public class CollectItemModel : ICollectItemModel
{
    private int _score;
    private ICollectItemEventBus _collectEventBus;
    
    public CollectItemModel(ICollectItemEventBus collectEventBus)
    {
        _collectEventBus = collectEventBus;
    }

    public void CollectItem(int value)
    {
        _score += value;
        _collectEventBus.CollectItem(_score);
        Debug.Log("CollectItem!!!");
    }
}
