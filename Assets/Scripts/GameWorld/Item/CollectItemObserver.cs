using System;
using UnityEngine;
using Zenject;

public class CollectItemObserver : IInitializable, IDisposable
{
    private ICollectItemEventBus _collectEventBus;
    private ISoundEventBus _soundEventBus;
    private IScoreItemView _scoreItemView;
    private VFXEventBus _vfxBus;

    public CollectItemObserver(ICollectItemEventBus collectEventBus,
                                ICollectItemModel collectItemModel,
                                ISoundEventBus soundEventBus,
                                IScoreItemView scoreItemView,
                                VFXEventBus vfxBus)
    {
        _soundEventBus = soundEventBus;
        _scoreItemView = scoreItemView;
        _collectEventBus = collectEventBus;
        _vfxBus = vfxBus;
    }
    public void Initialize() => _collectEventBus.OnCollectItem += CollectItem;
    public void Dispose() => _collectEventBus.OnCollectItem -= CollectItem;

    private void CollectItem(int value, Transform transform)
    {
        PlaySound(transform);
        PlayVFX(transform);
        UpdateView(value);
    }

    private void PlayVFX(Transform transform) => _vfxBus.Play(VFXType.CollectItem, transform.position);
    private void PlaySound(Transform transform) => _soundEventBus.Play(SoundType.CollectItem, transform.position);
    private void UpdateView(int value)
    {
        string scoreText = value.ToString();
        _scoreItemView.UpdateView(scoreText);       
    }
}
