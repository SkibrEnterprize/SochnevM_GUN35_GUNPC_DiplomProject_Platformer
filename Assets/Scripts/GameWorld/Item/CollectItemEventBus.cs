using System;
public class CollectItemEventBus : ICollectItemEventBus
{
    public event Action<int> OnCollectItem;

    public void CollectItem(int newValue) => OnCollectItem?.Invoke(newValue);
}
