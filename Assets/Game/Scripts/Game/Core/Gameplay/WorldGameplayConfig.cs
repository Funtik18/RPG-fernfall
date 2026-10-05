using Game.Core.Gameplay.World;
using Game.Core.Gameplay.World.Player;
using UnityEngine;

namespace Game.Core.Gameplay
{
    [ CreateAssetMenu( fileName = "WorldGameplayConfig", menuName = "Game/WorldGameplayConfig" ) ]
    public sealed class WorldGameplayConfig : ScriptableObject
    {
        [ field: SerializeField ] public LayerMask PlayerLayerMask { get; private set; }
        [ field: SerializeField ] public WorldObject WorldPrefab { get; private set; }
        [ field: SerializeField ] public PlayerConfig Player { get; private set; }
    }
}