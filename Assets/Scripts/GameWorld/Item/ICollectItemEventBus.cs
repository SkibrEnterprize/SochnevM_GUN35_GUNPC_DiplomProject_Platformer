
using System;
using UnityEngine;

public interface ICollectItemEventBus 
{
    public event Action<int, Transform> OnCollectItem;
    public void CollectItem(int newValue, Transform transform); 
}
