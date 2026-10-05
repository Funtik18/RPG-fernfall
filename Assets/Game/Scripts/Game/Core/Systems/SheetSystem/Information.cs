using UnityEngine;

namespace Game.Core.Systems.SheetSystem
{
    [ System.Serializable ]
    public sealed class Information
    {
        [ field: SerializeField ] public string Name { get; private set; }
    }
}