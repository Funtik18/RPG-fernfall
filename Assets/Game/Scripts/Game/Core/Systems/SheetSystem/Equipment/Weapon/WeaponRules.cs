using Game.Core.Gameplay.TBS;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Systems.SheetSystem
{
    [ CreateAssetMenu( fileName = "WeaponRules", menuName = "Game/Sheet/Inventory/WeaponRules" ) ]
    public sealed class WeaponRules : ScriptableObject
    {
        [ field: SerializeField ] public List< WeaponRule > Rules { get; private set; } = new();

        public int GetWeaponRuleAccuracy( UnitController attacker, UnitController defender ) => TryGetRule( attacker.Sheet.Equipment.Weapon, defender.Sheet.Equipment.Weapon, out var rule ) ? rule.Accuracy : 0;

        public int GetWeaponRuleDamage( UnitController attacker, UnitController defender ) => TryGetRule( attacker.Sheet.Equipment.Weapon, defender.Sheet.Equipment.Weapon, out var rule ) ? rule.Damage : 0;

        public bool IsWeak( Weapon attacker, Weapon defender )
        {
            WeaponRule rule = null;
            var accuracy = TryGetRule( attacker, defender, out rule ) ? rule.Accuracy : 0;
            var damage = TryGetRule( attacker, defender, out rule ) ? rule.Damage : 0;
            
            return accuracy < 0 || damage < 0;
        }
        
        private bool TryGetRule( Weapon attackerWeapon, Weapon defenderWeapon, out WeaponRule rule )
        {
            rule = null;

            var attackerType = attackerWeapon.Config.TriangleType;
            var defenderType = defenderWeapon.Config.TriangleType;

            foreach ( var candidate in Rules )
            {
                if ( candidate.Attacker != attackerType ) continue;
                if ( candidate.Defender != defenderType ) continue;

                rule = candidate;
                return true;
            }

            return false;
        }
    }

    [ System.Serializable ]
    public sealed class WeaponRule
    {
        [ field: SerializeField ] public WeaponType Attacker { get; private set; }
        [ field: SerializeField ] public WeaponType Defender { get; private set; }
        [ field: SerializeField ] public int Accuracy { get; private set; } = 10;
        [ field: SerializeField ] public int Damage { get; private set; } = 0;
    }
}
