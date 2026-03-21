using System;
using Zenject;

public class CollectItemModel : IInitializable, IDisposable, ICollectItemObserver
{
    private int _score;
    private ISoundEventBus _soundBus;


    public event Action<int> OnCountChanged;
    public int Score
    {
        get => _score;
        set
        {
           {
                _score++;
                OnCountChanged?.Invoke(_score);
            }
        }
    }
    public CollectItemModel(ISoundEventBus soundEventBus)
    {        
        _soundBus = soundEventBus;
    }

    public void Initialize()
    {
    }
    
    public void Dispose()
    {
    }


    public void CollectItem()
    {
        Score++;
        _soundBus.Play(SoundType.CollectItem);
    }
    
}
