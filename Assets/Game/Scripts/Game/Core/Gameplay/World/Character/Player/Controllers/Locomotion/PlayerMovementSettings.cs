using UnityEngine;

namespace Game.Core.Gameplay.World.Player
{
    [ System.Serializable ]
    public sealed class PlayerMovementSettings
    {
        [ field: SerializeField ] public float MoveSpeed { get; private set; } = 5f;
        [ field: SerializeField ] public float RotationSpeed { get; private set; } = 12f;
        [ field: SerializeField ] public float StopDistance { get; private set; } = 0.05f;
    }
}