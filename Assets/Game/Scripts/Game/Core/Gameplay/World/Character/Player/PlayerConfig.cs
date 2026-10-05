using UnityEngine;

namespace Game.Core.Gameplay.World.Player
{
    [ CreateAssetMenu( fileName = "PlayerConfig", menuName = "Game/PlayerConfig" ) ]
    public sealed class PlayerConfig : ScriptableObject
    {
        [ field: SerializeField ] public string UID { get; private set; }
        [ field: Space ]
        [ field: SerializeField ] public LayerMask PlayerLayerMask { get; private set; }
        [ field: SerializeField ] public LayerMask InteractableLayerMask { get; private set; }
        [ field: SerializeField ] public LayerMask GroundLayerMask { get; private set; }
        [ field: SerializeField ] public PlayerMovementSettings MovementSettings { get; private set; }
        [ field: SerializeField ] public PlayerCameraSettings CameraSettings { get; private set; }
        [ field: Space ]
        [ field: SerializeField ] public PlayerObject Prefab { get; private set; }
    }
}