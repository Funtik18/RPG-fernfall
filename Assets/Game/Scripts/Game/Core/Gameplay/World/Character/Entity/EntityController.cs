using Game.Core.Systems.CommandSystem;
using Game.Systems.StorageSystem;
using Game.Systems.StorageSystem.World;
using System;
using System.Linq;

namespace Game.Core.Gameplay.World.Entity
{
    public sealed class EntityController
    {
        public EntityObject View { get; }
        public EntityData Data { get; private set; }

        private readonly CommandFactory _commandFactory;
        private readonly DataHolder _dataHolder;
        
        public EntityController(
            EntityObject view,
            CommandFactory commandFactory,
            DataHolder dataHolder
            )
        {
            View = view ?? throw new ArgumentNullException( nameof(view) );
            _commandFactory = commandFactory ?? throw new ArgumentNullException( nameof(commandFactory) );
            _dataHolder = dataHolder ?? throw new ArgumentNullException( nameof(dataHolder) );
            
            View.SetController( this );
        }
        
        public void Initialize()
        {
            var worldData = _dataHolder.GameStorageData.ProgressData.Value.World;
            Data = worldData.Entities.FirstOrDefault( ( x ) => string.Equals( x.UID, View.UID, StringComparison.CurrentCultureIgnoreCase ) );
            if ( Data == null )
            {
                Data = new()
                {
                    UID = View.UID,
                    IsEnabled = true,
                };
                worldData.Entities.Add( Data );
            }
            else
            {
                View.gameObject.SetActive( Data.IsEnabled );
            }
        }
        
        public void Dispose()
        {
            
        }

        public void OnTriggerEnter()
        {
            if ( View.OnTriggerEnterCommand != null )
            {
                _commandFactory.Create( View.OnTriggerEnterCommand, View ).Execute();
            }
        }
        
        public void OnTriggerExit()
        {
            if ( View.OnTriggerExitCommand != null )
            {
                _commandFactory.Create( View.OnTriggerExitCommand, View ).Execute();
            }
        }
    }
}