using System;
using Game.Core.Gameplay.TBS;
using UnityEngine;
using Value;

namespace Game.Core.Systems.SheetSystem
{
    [ CreateAssetMenu( fileName = nameof(CleaveCombatArtConfig), menuName = "Game/Sheet/CombatArts/" + nameof(CleaveCombatArtConfig) ) ]
    public sealed class CleaveCombatArtConfig : CombatArtConfig
    {
        public override Type GetComboArtType() => typeof(CleaveCombatArt);
    }
    
    public sealed class CleaveCombatArt : CombatArt
    {
        private AddAttributeModifier _defenseIgnoreModifier;
        
        private readonly CleaveCombatArtConfig _config;

        public CleaveCombatArt( CleaveCombatArtConfig config ) : base( config )
        {
            _config = config ?? throw new ArgumentNullException( nameof(config) );
        }

        public override void OnBeforeAttack( UnitController attacker, UnitController defender )
        {
            base.OnBeforeAttack( attacker, defender );

            _defenseIgnoreModifier ??= new AddAttributeModifier( _config.X );

            if ( !attacker.Sheet.Stats.DefenseIgnore.Contains( _defenseIgnoreModifier ) )
            {
                attacker.Sheet.Stats.DefenseIgnore.AddModifier( _defenseIgnoreModifier );
            }
        }

        public override void OnAfterAttack( UnitController attacker, UnitController defender )
        {
            if ( _defenseIgnoreModifier != null && attacker.Sheet.Stats.DefenseIgnore.Contains( _defenseIgnoreModifier ) )
            {
                attacker.Sheet.Stats.DefenseIgnore.RemoveModifier( _defenseIgnoreModifier );
            }

            base.OnAfterAttack( attacker, defender );
        }
    }
}
