using System;
using Game.Core.Gameplay.TBS;
using Game.Managers.BattleManager;
using UnityEngine;
using Value;

namespace Game.Core.Systems.SheetSystem
{
    [ CreateAssetMenu( fileName = nameof(CalculatingShotSkillConfig), menuName = "Game/Sheet/Skills/" + nameof(CalculatingShotSkillConfig) ) ]
    public sealed class CalculatingShotSkillConfig : SkillConfig
    {
        public override Type GetSkillType() => typeof(CalculatingShotSkill);
    }
    
    public sealed class CalculatingShotSkill : Skill
    {
        private AddAttributeModifier _criticalModifier;

        private readonly CalculatingShotSkillConfig _config;
        private readonly BattleManager _battleManager;

        public CalculatingShotSkill(
            CalculatingShotSkillConfig config,
            BattleManager battleManager
            ) : base( config )
        {
            _config = config ?? throw new ArgumentNullException( nameof(config) );
            _battleManager = battleManager ?? throw new ArgumentNullException( nameof(battleManager) );
        }

        public override void OnBeforeHit( Sheet attacker, Sheet defender, bool defenderCanCounterattack, bool attackedFromDistance )
        {
            if ( !TryGetUnit( defender, out var defenderUnit ) ) return;
            if ( defenderUnit.HasMovedDuringLastTurn ) return;

            _criticalModifier ??= new AddAttributeModifier( _config.X );

            if ( !attacker.Stats.Critical.Contains( _criticalModifier ) )
            {
                attacker.Stats.Critical.AddModifier( _criticalModifier );
            }
        }

        public override void OnAfterHit( Sheet attacker, Sheet defender, bool isHit )
        {
            RemoveModifier( attacker );
        }

        public override void Dispose( Sheet sheet )
        {
            RemoveModifier( sheet );
        }

        private bool TryGetUnit( Sheet sheet, out UnitController unit )
        {
            unit = _battleManager.BattleController.Battle.Units.Find( ( x ) => x.Sheet == sheet );
            return unit != null;
        }

        private void RemoveModifier( Sheet sheet )
        {
            if ( _criticalModifier == null ) return;

            if ( sheet.Stats.Critical.Contains( _criticalModifier ) )
            {
                sheet.Stats.Critical.RemoveModifier( _criticalModifier );
            }
        }
    }
}
