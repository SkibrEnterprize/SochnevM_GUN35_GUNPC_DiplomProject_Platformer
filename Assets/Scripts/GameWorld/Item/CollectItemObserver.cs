using System;
using Zenject;

public class CollectItemObserver : IInitializable, IDisposable
{
    private ICollectItemEventBus _collectEventBus;
    private ISoundEventBus _soundEventBus;
    private IScoreItemView _scoreItemView;

    public CollectItemObserver(ICollectItemEventBus collectEventBus,
                                ICollectItemModel collectItemModel,
                                ISoundEventBus soundEventBus,
                                IScoreItemView scoreItemView)
    {
        _soundEventBus = soundEventBus;
        _scoreItemView = scoreItemView;
        _collectEventBus = collectEventBus;
    }
    public void Initialize()
    {
        _collectEventBus.OnCollectItem += CollectItem;
    }
    public void Dispose()
    {
        _collectEventBus.OnCollectItem -= CollectItem;
    }

    private void CollectItem(int value)
    {
        PlaySound();
        UpdateView(value);
    }

    private void PlaySound() => _soundEventBus.Play(SoundType.CollectItem);
    
    private void UpdateView(int value)
    {
        string scoreText = value.ToString();
        _scoreItemView.UpdateView(scoreText);       
    }
}
