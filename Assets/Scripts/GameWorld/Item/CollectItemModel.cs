using System;
using Zenject;

public class CollectItemModel : IInitializable, IDisposable, ICollectItemObserver
{
    private int _score;
    private readonly SoundLibrary _soundLibrary;

    private LevelFinishConfig _config;

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
    public CollectItemModel(LevelFinishConfig levelFinishConfig, SoundLibrary soundLibrary)
    {        
        _config = levelFinishConfig;
        _soundLibrary = soundLibrary;
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
        _soundLibrary.RequestPlay(SoundType.CollectItem);
    }
    
}
