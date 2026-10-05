using Cysharp.Threading.Tasks;
using Game.Core.Gameplay.TBS;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Systems.SheetSystem
{
    [ CreateAssetMenu( fileName = nameof(ThreshCombatArtConfig), menuName = "Game/Sheet/CombatArts/" + nameof(ThreshCombatArtConfig) ) ]
    public sealed class ThreshCombatArtConfig : CombatArtConfig
    {
        public override Type GetComboArtType() => typeof( ThreshCombatArt );
    }

    public sealed class ThreshCombatArt : CombatArt
    {
        private readonly List< UnitController > _targets = new();
        
        private readonly BattleAttackExecutor _attackExecutor;
        private readonly BattleCombatTargetingController _targetingController;
        private readonly BattleGridController _gridController;
        private readonly BattlePathfinder _pathfinder;

        public ThreshCombatArt(
            ThreshCombatArtConfig config,
            BattleAttackExecutor attackExecutor,
            BattleCombatTargetingController targetingController,
            BattleGridController gridController,
            BattlePathfinder pathfinder
            ) : base( config )
        {
            _attackExecutor = attackExecutor ?? throw new ArgumentNullException( nameof(attackExecutor) );
            _targetingController = targetingController ?? throw new ArgumentNullException( nameof(targetingController) );
            _gridController = gridController ?? throw new ArgumentNullException( nameof(gridController) );
            _pathfinder = pathfinder ?? throw new ArgumentNullException( nameof(pathfinder) );
        }

        public override async UniTask Apply( UnitController attacker, UnitController defender )
        {
            CollectTargets( attacker, defender );

            foreach ( var target in _targets )
            {
                if ( !attacker.IsAlive() ) break;
                if ( !target.IsAlive() ) continue;

                _attackExecutor.ApplyAdditionalAttackDamage( attacker, target, false ).Forget();
            }
        }

        private void CollectTargets( UnitController attacker, UnitController defender )
        {
            _targets.Clear();

            var weapon = attacker?.Sheet?.Equipment?.Weapon;
            if ( weapon == null ) return;
            if ( !_gridController.Registry.TryGetUnitCell( attacker, out var attackerCell ) ) return;

            foreach ( var cell in _pathfinder.GetAttackCells( attackerCell, attacker, 1 ) )
            {
                var unit = _gridController.Registry.GetUnit( cell );
                if ( unit == null ) continue;
                if ( unit == defender ) continue;
                if ( !unit.IsAlive() ) continue;

                _targets.Add( unit );
            }
        }
    }
}
