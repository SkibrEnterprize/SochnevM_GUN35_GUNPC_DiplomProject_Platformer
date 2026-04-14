using System;
using UnityEngine;
public class CollectItemEventBus : ICollectItemEventBus
{
    public event Action<int, Transform> OnCollectItem;

    public void CollectItem(int newValue, Transform transform) 
        => OnCollectItem?.Invoke(newValue, transform);
}
