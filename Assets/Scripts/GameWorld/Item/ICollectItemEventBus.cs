
using System;

public interface ICollectItemEventBus 
{
    public event Action<int> OnCollectItem;
    public void CollectItem(int newValue); 
}
