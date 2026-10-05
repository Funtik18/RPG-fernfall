using Game.Systems.StorageSystem;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Systems.SheetSystem
{
    public sealed class WeaponFactory
    {
        public IEnumerable< Weapon > GetWeapons( Inventory inventory )
        {
            foreach ( var item in inventory.Items )
            {
                if ( TryCreateWeapon( item, out var weapon ) )
                {
                    yield return weapon;
                }
            }
        }

        private bool TryCreateWeapon( InventoryItem item, out Weapon weapon )
        {
            weapon = null;

            if ( item.Config is not WeaponItemConfig config ) return false;

            var data = GetWeaponData( item.Data, config );
            weapon = new( config );
            weapon.SetData( data );
            weapon.OnChanged += ( changedWeapon ) => CommitWeaponData( changedWeapon, item.Data );

            CommitWeaponData( weapon, item.Data );

            return true;
        }

        private WeaponItemData GetWeaponData( InventoryItemData inventoryItemData, WeaponItemConfig config )
        {
            if ( !string.IsNullOrWhiteSpace( inventoryItemData?.Json ) )
            {
                var data = JsonUtility.FromJson< WeaponItemData >( inventoryItemData.Json );
                if ( data != null )
                {
                    return data;
                }
            }

            return new()
            {
                Uses = config.Uses
            };
        }

        private void CommitWeaponData( Weapon weapon, InventoryItemData inventoryItemData )
        {
            if ( weapon == null ) return;
            if ( inventoryItemData == null ) return;

            weapon.Commit();
            inventoryItemData.Json = JsonUtility.ToJson( weapon.Data );
        }
    }
}
