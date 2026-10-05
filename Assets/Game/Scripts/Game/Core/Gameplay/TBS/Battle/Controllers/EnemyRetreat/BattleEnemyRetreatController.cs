using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Gameplay.TBS
{
    public sealed class BattleEnemyRetreatController
    {
        private readonly HashSet< UnitController > _retreatingUnits = new();

        private readonly TBSGameplayConfig _config;
        private readonly BattleGridController _gridController;
        private readonly BattleCombatController _combatController;

        private Battle _battle;

        public BattleEnemyRetreatController(
            TBSGameplayConfig config,
            BattleGridController gridController,
            BattleCombatController combatController
            )
        {
            _config = config ?? throw new ArgumentNullException( nameof(config) );
            _gridController = gridController ?? throw new ArgumentNullException( nameof(gridController) );
            _combatController = combatController ?? throw new ArgumentNullException( nameof(combatController) );
        }

        public void Initialize( Battle battle )
        {
            Dispose();

            _battle = battle ?? throw new ArgumentNullException( nameof(battle) );
            _combatController.OnUnitKilled += UnitKilledHandler;
        }

        public void Dispose()
        {
            _combatController.OnUnitKilled -= UnitKilledHandler;
            _battle = null;
            _retreatingUnits.Clear();
        }

        public bool IsRetreating( UnitController unit ) => _retreatingUnits.Contains( unit );

        public void CompleteRetreat( UnitController unit )
        {
            _retreatingUnits.Remove( unit );
        }

        private void UnitKilledHandler( UnitController attacker, UnitController killedUnit )
        {
            if ( killedUnit == null ) return;
            if ( _battle == null ) return;

            _retreatingUnits.Remove( killedUnit );

            var killedUnitTeam = _battle.GetTeam( killedUnit );
            if ( !killedUnitTeam.IsEnemyTeam() ) return;

            foreach ( var unit in killedUnitTeam.Units )
            {
                if ( unit == killedUnit ) continue;
                if ( !unit.IsAlive() ) continue;
                if ( !_gridController.Registry.TryGetUnitCell( unit, out _ ) ) continue;

                TryRequestRetreat( unit );
            }
        }

        private void TryRequestRetreat( UnitController unit )
        {
            if ( _retreatingUnits.Contains( unit ) ) return;
            if ( UnityEngine.Random.value >= Mathf.Clamp01( _config.EnemyRetreatChance / 100f ) ) return;

            _retreatingUnits.Add( unit );
            
            Debug.LogError( "Unit " + unit.Sheet.Information.Name + " is retreating!", unit.View.gameObject );
        }
    }
}
