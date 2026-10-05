using Game.Core.Gameplay.TBS;
using System;

namespace Game.Core.Systems.SheetSystem
{
    public static class WeaponFamilyConsts
    {
        public const string Bees = "Bees";
        public const string Unarmed = "Unarmed";
        
        public static bool IsBeesWeaponEquipped( this UnitController attacker )
        {
            var family = attacker.Sheet.Equipment.Weapon.Config.Family;
            return string.Equals( family.Name, Bees, StringComparison.InvariantCultureIgnoreCase );
        }
    }
}