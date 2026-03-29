using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "VFX/VFX Library")]
public class VFXLibrary : ScriptableObject
{
    public List<VFXMapping> effects;

    public VFXEvent GetEvent(VFXType type)
    {
        return effects.Find(e => e.type == type).vfxEvent;
    }
}