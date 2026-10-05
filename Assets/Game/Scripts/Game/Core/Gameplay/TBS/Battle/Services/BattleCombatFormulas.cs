using System;
using Game.Core.Systems.SheetSystem;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Core.Gameplay.TBS
{
    public static class BattleCombatFormulas
    {
        public static int GetDamage( UnitController attacker, UnitController defender, bool isCritical, WeaponRules weaponRules )
        {
            int damage = 0;
            if ( attacker.Sheet.Equipment.Weapon.Config.TriangleType == WeaponType.Craft )
            {
                var resistIgnorePercent = Mathf.Clamp( attacker.Sheet.Stats.ResistIgnore.TotalValue, 0f, 100f );
                var defense = defender.Sheet.Stats.Resist.TotalValue * ( 1f - resistIgnorePercent / 100f );
                damage = Mathf.Max( 1, (int)( attacker.GetCraftAttack() - defense ) );
            }
            else
            {
                var defenseIgnorePercent = Mathf.Clamp( attacker.Sheet.Stats.DefenseIgnore.TotalValue, 0f, 100f );
                var defense = defender.Sheet.Stats.Defense.TotalValue * ( 1f - defenseIgnorePercent / 100f );
                damage = Mathf.Max( 1, (int)( attacker.GetPhysicalAttack() - defense ) );
            }

            if ( isCritical )
            {
                damage = Mathf.RoundToInt( damage * 3f );
            }

            return Mathf.Max( 1, damage + (int)attacker.Sheet.Stats.ExtraDamage.TotalValue + weaponRules.GetWeaponRuleDamage( attacker, defender ) );
        }
        
        public static bool IsHit( UnitController attacker, UnitController defender, WeaponRules weaponRules, BattleCell defenderCell )
        {
            int hitRate = CalculateHitChance();
            int roll1 = Random.Range( 0, 100 );
            int roll2 = Random.Range( 0, 100 );

            return ( ( roll1 + roll2 ) / 2f ) < hitRate;
            
            int CalculateHitChance()
            {
                int hit = attacker.GetHitRate() + weaponRules.GetWeaponRuleAccuracy( attacker, defender );
                bool isIgnoreAvoid = attacker.Skills.Contains< SteadyHandSkill >() || attacker.Skills.Contains< CanNotBeFooledSkill >();
                int avoid = defender.GetAvoidRate( defenderCell, isIgnoreAvoid );

                return Mathf.Clamp( hit - avoid, 0, 100 );
            }
        }

        public static bool IsCriticalHit( UnitController attacker, UnitController defender )
        {
            int criticalChance = CalculateCriticalChance();
            int roll = Random.Range( 0, 100 );

            return roll < criticalChance;

            int CalculateCriticalChance()
            {
                int criticalRate = attacker.GetCriticalRate();
                int criticalAvoid = defender.GetCriticalAvoidRate();

                return Mathf.Clamp( criticalRate - criticalAvoid, 0, 100 );
            }
        }

        public static int GetAttack( this UnitController unit )
        {
            var sheet = unit.Sheet;

            if ( sheet.Equipment.Weapon.Config.TriangleType == WeaponType.Craft )
            {
                return unit.GetCraftAttack();
            }
            return unit.GetPhysicalAttack();
        }

        public static int GetHitRate( this UnitController unit )
        {
            var sheet = unit.Sheet;

            if ( sheet.Equipment.Weapon.Config.TriangleType == WeaponType.Craft )
            {
                return unit.GetCraftHitRate();
            }
            return unit.GetPhysicalHitRate();
        }

        public static int GetAvoidRate( this UnitController unit, BattleCell cell = null, bool ignoreTerrainAvoidBonus = false )
        {
            var sheet = unit.Sheet;
            var terrainAvoid = GetTerrainAvoidRate();

            if ( sheet.Equipment.Weapon.Config.TriangleType == WeaponType.Craft )
            {
                return unit.GetCraftAvoidRate() + (int)terrainAvoid;
            }
            return unit.GetPhysicalAvoidRate() + (int)terrainAvoid;
            
            float GetTerrainAvoidRate()
            {
                var avoid = cell?.Avoid ?? 0f;
                if ( ignoreTerrainAvoidBonus && avoid > 0f ) return 0f;

                return avoid;
            }
        }
        
        public static int GetCriticalRate( this UnitController unit )
        {
            var sheet = unit.Sheet;
            
            return (int)( Math.Max( 0, ( sheet.Stats.Dexterity.TotalValue - 4 ) * 0.5f ) + sheet.Equipment.Weapon.Critical + sheet.Stats.Critical.TotalValue );
        }

        public static int GetCriticalAvoidRate( this UnitController unit )
        {
            var sheet = unit.Sheet;
            
            return (int)( sheet.Stats.Luck.TotalValue * 0.5f );
        }
        
        /// PHYSICAL
        
        public static int GetPhysicalAttack( this UnitController unit )
        {
            var sheet = unit.Sheet;
            
            return (int)( sheet.Stats.Strength.TotalValue + sheet.Equipment.Weapon.Might );
        }

        public static int GetPhysicalHitRate( this UnitController unit )
        {
            var sheet = unit.Sheet;
            
            return (int)( sheet.Stats.Dexterity.TotalValue * 1.5f + sheet.Stats.Luck.TotalValue * 0.5f + sheet.Equipment.Weapon.Hit + sheet.Stats.Hit.TotalValue );
        }

        public static int GetPhysicalAvoidRate( this UnitController unit )
        {
            var sheet = unit.Sheet;
            var stats = sheet.Stats;

            var weight = sheet.GetWeight();
            
            return (int)( stats.Speed.TotalValue - (Mathf.Max( 0, weight - stats.Strength.TotalValue )) * 1.5f + stats.Luck.TotalValue * 0.5f + sheet.Equipment.Weapon.Avoid + stats.Avoid.TotalValue );
        }
        
        /// CRAFT
        
        public static int GetCraftAttack( this UnitController unit )
        {
            var sheet = unit.Sheet;

            return (int)( sheet.Stats.Craft.Value + sheet.Equipment.Weapon.Might );
        }
        
        public static int GetCraftHitRate( this UnitController unit )
        {
            var sheet = unit.Sheet;
            
            return (int)( sheet.Stats.Craft.Value + sheet.Stats.Dexterity.Value + sheet.Equipment.Weapon.Hit + sheet.Stats.Hit.TotalValue );
        }
        
        public static int GetCraftAvoidRate( this UnitController unit )
        {
            var sheet = unit.Sheet;
            
            return (int)( sheet.Stats.Resist.TotalValue * 1.5f + sheet.Stats.Luck.TotalValue * 0.5f + sheet.Equipment.Weapon.Avoid + sheet.Stats.Avoid.TotalValue );
        }
    }
}
