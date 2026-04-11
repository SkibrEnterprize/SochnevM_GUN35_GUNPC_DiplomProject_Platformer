using UnityEngine;
using Zenject;

public class VFXObserver : IInitializable, System.IDisposable
{
    private readonly VFXEventBus _bus;
    private readonly IVFXSystem _vfxSystem;

    public VFXObserver(VFXEventBus bus, IVFXSystem vfxSystem)
    {
        _bus = bus;
        _vfxSystem = vfxSystem;
    }

    public void Initialize() => _bus.OnVFXRequested += Notify;
    public void Dispose() => _bus.OnVFXRequested -= Notify;

    private void Notify(VFXType type, 
        Vector3 pos,       
        Quaternion rot, 
        Transform parent) =>
            _vfxSystem.Play(type, 
                pos,                
                rot, 
                parent);
}