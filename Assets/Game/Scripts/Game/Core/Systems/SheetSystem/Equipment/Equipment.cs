using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Core.Systems.SheetSystem
{
    public sealed class Equipment
    {
        public Weapon BaseWeapon { get; }

        public Weapon Weapon { get; private set; }

        private readonly List< Weapon > _weapons = new();

        public Equipment( Weapon baseWeapon, IEnumerable< Weapon > weapons = null )
        {
            BaseWeapon = baseWeapon ?? throw new ArgumentNullException( nameof(baseWeapon) );

            if ( weapons != null )
            {
                _weapons.AddRange( weapons );
            }

            Weapon = _weapons.FirstOrDefault( x => x.CanUse ) ?? BaseWeapon;
        }
        
        public void EquipWeapon( Weapon weapon )
        {
            Weapon = weapon ?? throw new ArgumentNullException( nameof(weapon) );
        }

        public void UnequipWeapon()
        {
            Weapon = BaseWeapon;
        }

        public void ResetSpecialUses()
        {
            BaseWeapon.ResetSpecialUses();

            foreach ( var weapon in _weapons )
            {
                weapon.ResetSpecialUses();
            }

            if ( !Weapon.CanUse )
            {
                UnequipWeapon();
            }
        }
        
        public IEnumerable< Weapon > GetWeapons()
        {
            yield return BaseWeapon;

            foreach ( var weapon in _weapons.Where( x => x.CanUse ) )
            {
                yield return weapon;
            }
        }
    }
}
