using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Systems.SheetSystem
{
    [ CreateAssetMenu( fileName = "Item", menuName = "Game/Sheet/Inventory/Database" ) ]
    public sealed class InventoryDatabase : ScriptableObject
    {
        [ field: SerializeField ] public List< ItemConfig > AllItems { get; private set; }
        [ field: SerializeField ] public WeaponItemConfig BaseWeapon { get; private set; }

        private Dictionary< string, ItemConfig > _items;
        
        public ItemConfig GetItem( string uid )
        {
            if ( _items == null )
            {
                _items = new();
                foreach ( var item in AllItems )
                {
                    _items.Add( item.UID, item );
                }
            }
            return _items[ uid ];
        }
    }
}