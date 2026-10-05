using UnityEngine;

namespace Game.Core.Gameplay.World.Player
{
    [ System.Serializable ]
    public sealed class PlayerAvatar
    {
        [ field: SerializeField ] public TriggerCollider TriggerCollider { get; private set; }
    }
}