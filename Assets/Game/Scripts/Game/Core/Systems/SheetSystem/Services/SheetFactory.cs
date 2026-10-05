using System;
using Game.Systems.StorageSystem;

namespace Game.Core.Systems.SheetSystem
{
    public sealed class SheetFactory
    {
        private readonly WeaponFactory _weaponFactory = new();
        
        private readonly InventoryDatabase _database;
        
        public SheetFactory( InventoryDatabase database )
        {
            _database = database ?? throw new ArgumentNullException( nameof(database) );
        }

        public Sheet Create( SheetSettings settings, InventoryData inventoryData = null )
        {
            Information information = settings.Information;
            FractionConfig fractionConfig = settings.Fraction;
            ClassConfig classConfig = settings.Class;
            Stats stats = new( classConfig.Stats );
            Inventory inventory = new( settings.Inventory );
            if ( inventoryData != null )
            {
                inventory.SetData( inventoryData, _database );
            }

            var weapons = _weaponFactory.GetWeapons( inventory );
            Equipment equipment = new( new Weapon( _database.BaseWeapon, true ), weapons );
            
            return new( information, fractionConfig, classConfig, stats, inventory, equipment );
        }
    }
}
