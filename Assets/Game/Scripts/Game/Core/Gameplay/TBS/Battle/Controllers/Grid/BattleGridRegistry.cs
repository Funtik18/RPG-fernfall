using System;
using System.Collections.Generic;

namespace Game.Core.Gameplay.TBS
{
    public sealed class BattleGridRegistry
    {
        public event Action< UnitController, BattleCell > OnUnitPlaced;
        public event Action< UnitController, BattleCell, BattleCell > OnUnitMoved;
        public event Action< UnitController, BattleCell > OnUnitRemoved;

        private readonly Dictionary< UnitController, BattleCell > _unitCells = new();
        private readonly Dictionary< BattleCell, UnitController > _cellUnits = new();

        public BattleCell GetUnitCell( UnitController unit ) => _unitCells[ unit ];

        public bool TryGetUnitCell( UnitController unit, out BattleCell cell )
        {
            if ( unit == null )
            {
                cell = null;
                return false;
            }

            return _unitCells.TryGetValue( unit, out cell );
        }

        public UnitController GetUnit( BattleCell cell ) => _cellUnits.GetValueOrDefault( cell );

        public bool IsOccupied( BattleCell cell ) => _cellUnits.ContainsKey( cell );

        public void Place( UnitController unit, BattleCell cell )
        {
            if ( IsOccupied( cell ) )
                throw new InvalidOperationException( "[Battle] Cell is occupied." );

            if ( _unitCells.ContainsKey( unit ) )
                throw new InvalidOperationException( "[Battle] Unit already placed." );

            _unitCells.Add( unit, cell );
            _cellUnits.Add( cell, unit );

            OnUnitPlaced?.Invoke( unit, cell );
        }

        public void Move( UnitController unit, BattleCell target )
        {
            if ( !_unitCells.TryGetValue( unit, out var current ) )
                throw new InvalidOperationException( "[Battle] Unit is not placed." );

            if ( IsOccupied( target ) )
                throw new InvalidOperationException( "[Battle] Target cell is occupied." );

            _cellUnits.Remove( current );

            _unitCells[ unit ] = target;
            _cellUnits[ target ] = unit;

            OnUnitMoved?.Invoke( unit, current, target );
        }

        public void Remove( UnitController unit )
        {
            if ( !_unitCells.Remove( unit, out var cell ) )
                return;

            _cellUnits.Remove( cell );

            OnUnitRemoved?.Invoke( unit, cell );
        }
    }
}
