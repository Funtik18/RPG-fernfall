using Game.Core.Systems.CommandSystem;
using System;

namespace Game.Core.Gameplay.World.Entity
{
    public abstract class EntityCommandPresenter< T > : CommandPresenter< T >
        where T : CommandComponent
    {
        protected readonly EntityObject _entity;
        
        protected EntityCommandPresenter(
            EntityObject entity,
            T component
            ) : base( component )
        {
            _entity = entity ?? throw new ArgumentNullException( nameof(entity) );
        }
    }
}