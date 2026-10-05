using Game.Core.Gameplay.World.Player;
using Game.Core.Systems.SheetSystem;
using Game.Managers.PartyManager;
using System;
using Zenject;

namespace Game.Core.Gameplay.TBS
{
    public sealed class UnitFactory
    {
        private readonly DiContainer _diContainer;
        private readonly PartyManager _partyManager;
        private readonly SheetFactory _sheetFactory;
        
        public UnitFactory(
            DiContainer diContainer,
            PartyManager partyManager,
            SheetFactory sheetFactory
            )
        {
            _diContainer = diContainer ?? throw new ArgumentNullException( nameof(diContainer) );
            _partyManager = partyManager ?? throw new ArgumentNullException( nameof(partyManager) );
            _sheetFactory = sheetFactory ?? throw new ArgumentNullException( nameof(sheetFactory) );
        }
        
        public UnitController Create( UnitConfig config )
        {
            UnitObject view = null;
            
            if ( config is PlayerUnitConfig player )
            {
                view = _diContainer.InstantiatePrefabForComponent< UnitObject >( player.Prefab );
                view.Controller.SetSheet( _partyManager.GetSheet( player.Index ) );
            }
            else if ( config is EnemyUnitConfig enemy )
            {
                view = _diContainer.InstantiatePrefabForComponent< UnitObject >( enemy.Prefab );
                view.Controller.SetSheet( _sheetFactory.Create( enemy.SheetSettings ) );
            }
            view.Controller.SetConfig( config );
            
            return view.Controller;
        }
    }
}
