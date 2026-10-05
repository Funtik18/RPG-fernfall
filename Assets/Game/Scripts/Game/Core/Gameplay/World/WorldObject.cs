using Game.Core.Gameplay.World.Entity;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Gameplay.World
{
    public sealed class WorldObject : MonoBehaviour
    {
        [ field: SerializeField ] public Transform PlayerSpawnPoint { get; private set; }
        [ field: Space ]
        [ field: SerializeField ] public List< EntityObject > Entities { get; private set; } = new();

        public WorldController Controller { get; private set; }
        
        public void SetController( WorldController controller )
        {
            Controller = controller;
        }

        private void OnDrawGizmos()
        {
            if ( PlayerSpawnPoint == null ) return;
            
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere( PlayerSpawnPoint.position, 1f );
        }
    }
}