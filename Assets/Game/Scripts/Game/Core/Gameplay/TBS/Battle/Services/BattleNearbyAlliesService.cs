using Game.Core.Systems.SheetSystem;
using System;
using System.Collections.Generic;

namespace Game.Core.Gameplay.TBS
{
    public sealed class BattleNearbyAlliesService
    {
        private readonly BattleGridController _gridController;
        private readonly BattlePathfinder _pathfinder;

        public BattleNearbyAlliesService(
            BattleGridController gridController,
            BattlePathfinder pathfinder
            )
        {
            _gridController = gridController ?? throw new ArgumentNullException( nameof(gridController) );
            _pathfinder = pathfinder ?? throw new ArgumentNullException( nameof(pathfinder) );
        }

        public bool HasNearbyAllies( Battle battle, Sheet sheet )
        {
            return TryHasNearbyAllies( battle, sheet, out var hasNearbyAllies ) && hasNearbyAllies;
        }

        public bool TryHasNearbyAllies( Battle battle, Sheet sheet, out bool hasNearbyAllies )
        {
            hasNearbyAllies = false;

            if ( battle == null ) return false;
            if ( sheet == null ) return false;

            foreach ( var unit in battle.Units )
            {
                if ( unit.Sheet != sheet ) continue;

                return TryHasNearbyAllies( battle, unit, out hasNearbyAllies );
            }

            return false;
        }

        public bool HasNearbyAllies( Battle battle, UnitController unit )
        {
            return TryHasNearbyAllies( battle, unit, out var hasNearbyAllies ) && hasNearbyAllies;
        }

        public bool TryHasNearbyAllies( Battle battle, UnitController unit, out bool hasNearbyAllies )
        {
            hasNearbyAllies = false;

            if ( unit.Sheet.IsDead ) return false;

            var team = battle.GetTeam( unit );
            if ( team == null ) return false;
            if ( !_gridController.Registry.TryGetUnitCell( unit, out var unitCell ) ) return false;

            foreach ( var cell in unitCell.Neighbours )
            {
                var nearbyUnit = _gridController.Registry.GetUnit( cell );
                if ( nearbyUnit == null ) continue;
                if ( nearbyUnit == unit ) continue;
                if ( nearbyUnit.Sheet.IsDead ) continue;
                if ( !team.Contains( nearbyUnit ) ) continue;

                hasNearbyAllies = true;
                return true;
            }

            return true;
        }

        public bool TryGetAlliesWithinTiles( Battle battle, Sheet sheet, int tiles, List< UnitController > allies )
        {
            allies.Clear();

            foreach ( var unit in battle.Units )
            {
                if ( unit.Sheet != sheet ) continue;

                return TryGetAlliesWithinTiles( battle, unit, tiles, allies );
            }

            return false;
        }

        public bool TryGetAlliesWithinTiles( Battle battle, UnitController unit, int tiles, List< UnitController > allies, bool allowDeadSource = false )
        {
            allies.Clear();

            if ( !allowDeadSource && unit.Sheet.IsDead ) return false;

            var team = battle.GetTeam( unit );
            if ( team == null ) return false;
            if ( !_gridController.Registry.TryGetUnitCell( unit, out var unitCell ) ) return false;

            if ( tiles <= 0 ) return true;

            var cellsInRange = new HashSet< BattleCell >( _pathfinder.GetAttackCells( unitCell, unit, tiles ) );
            foreach ( var ally in team.Units )
            {
                if ( ally == unit ) continue;
                if ( ally.Sheet.IsDead ) continue;
                if ( !_gridController.Registry.TryGetUnitCell( ally, out var allyCell ) ) continue;
                if ( !cellsInRange.Contains( allyCell ) ) continue;

                allies.Add( ally );
            }

            return true;
        }

        public bool TryGetAlliesFromTiles( Battle battle, UnitController unit, int tiles, List< UnitController > allies, bool allowDeadSource = false )
        {
            allies.Clear();

            if ( !allowDeadSource && unit.Sheet.IsDead ) return false;

            var team = battle.GetTeam( unit );
            if ( team == null ) return false;
            if ( !_gridController.Registry.TryGetUnitCell( unit, out var unitCell ) ) return false;

            var cellsBeforeRange = tiles > 0
                ? new HashSet< BattleCell >( _pathfinder.GetAttackCells( unitCell, unit, tiles - 1 ) )
                : new HashSet< BattleCell >();

            foreach ( var ally in team.Units )
            {
                if ( ally == unit ) continue;
                if ( ally.Sheet.IsDead ) continue;
                if ( !_gridController.Registry.TryGetUnitCell( ally, out var allyCell ) ) continue;
                if ( cellsBeforeRange.Contains( allyCell ) ) continue;

                allies.Add( ally );
            }

            return true;
        }
    }
}
