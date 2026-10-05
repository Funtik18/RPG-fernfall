using UnityEngine;

namespace Game.Core.Gameplay.World.Player
{
    [ System.Serializable ]
    public sealed class PlayerCameraSettings
    {
        [ field: SerializeField ] public float Distance { get; private set; } = 12f;
        [ field: SerializeField ] public float MinDistance { get; private set; } = 5f;
        [ field: SerializeField ] public float MaxDistance { get; private set; } = 18f;
        [ field: SerializeField ] public float ZoomSpeed { get; private set; } = 2f;
        [ field: SerializeField ] public float ZoomSmoothTime { get; private set; } = 0.08f;
        [ field: Space ]
        [ field: SerializeField ] public float Pitch { get; private set; } = 50f;
        [ field: SerializeField ] public float Yaw { get; private set; } = 0f;
        [ field: SerializeField ] public float FollowSmoothTime { get; private set; } = 0.12f;
        [ field: SerializeField ] public float FreeMoveSpeed { get; private set; } = 10f;

    }
}