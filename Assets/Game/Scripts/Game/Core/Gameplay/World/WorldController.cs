using Game.Systems.StorageSystem;
using System;

namespace Game.Core.Gameplay.World
{
    public sealed class WorldController : IMemento
    {
        public WorldObject View { get; }

        private readonly WorldGameplayConfig _worldGameplayConfig;
        
        public WorldController(
            WorldObject view,
            WorldGameplayConfig worldGameplayConfig
            )
        {
            View = view ?? throw new ArgumentNullException( nameof(view) );
            _worldGameplayConfig = worldGameplayConfig ?? throw new ArgumentNullException( nameof(worldGameplayConfig) );
            View.SetController( this );
        }

        public void Initialize()
        {
            for ( var i = 0; i < View.Entities.Count; i++ )
            {
                View.Entities[ i ].Initialize();
            }
        }
        
        public void Dispose()
        {
            for ( var i = 0; i < View.Entities.Count; i++ )
            {
                View.Entities[ i ].Dispose();
            }
        }

        public void Commit()
        {
            
        }

        public void RestoreCommit()
        {
            
        }
    }
}