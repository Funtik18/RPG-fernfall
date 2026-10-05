using UnityEngine;

namespace Game.VFX
{
    [ System.Serializable ]
    public sealed class VFXSettings
    {
        [ field: SerializeField ] public VFXFloatingTextObject FloatingTextPrefab { get; private set; } 
    }
}