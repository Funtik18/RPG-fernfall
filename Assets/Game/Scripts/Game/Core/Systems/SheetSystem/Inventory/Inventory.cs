using Game.Systems.StorageSystem;
using System;
using System.Collections.Generic;

namespace Game.Core.Systems.SheetSystem
{
    public sealed class Inventory
    {
        public event Action OnChanged;

        public List< InventoryItem > Items { get; } = new();

        public InventoryData Data { get; private set; }

        public Inventory( InventorySettings settings )
        {
            Data = new();

            foreach ( var item in settings.Items )
            {
                AddItem( item, false );
            }
        }

        public void SetData( InventoryData inventoryData, InventoryDatabase service )
        {
            Data = inventoryData ?? new();
            Items.Clear();

            for ( int i = 0; i < Data.Items.Count; i++ )
            {
                var data = Data.Items[ i ];
                var config = service.GetItem( data.UID );
                var item = new InventoryItem( config );
                item.SetData( data );
                Items.Add( item );
            }
        }

        public void AddItem( ItemConfig config, bool notify = true )
        {
            Data ??= new();

            var item = GetItem( config.UID );
            if ( item == null )
            {
                var data = new InventoryItemData
                {
                    UID = config.UID
                };
                Data.Items.Add( data );

                item = new( config );
                item.SetData( data );
                Items.Add( item );
            }
            else
            {
                item.Quantity++;
            }

            if ( notify )
            {
                OnChanged?.Invoke();
            }
        }

        public void RemoveItem( ItemConfig config ) => RemoveItem( config.UID );

        public void RemoveItem( string uid )
        {
            var item = GetItem( uid );
            RemoveItem( item );
        }

        public void RemoveItem( InventoryItem item )
        {
            if ( item == null ) return;

            Data?.Items.Remove( item.Data );
            Items.Remove( item );

            OnChanged?.Invoke();
        }

        public bool ContainsItem( ItemConfig config ) => ContainsItem( config.UID );
        public bool ContainsItem( string uid ) => GetItem( uid ) != null;

        public InventoryItem GetItem( ItemConfig config ) => GetItem( config.UID );

        public InventoryItem GetItem( string uid )
        {
            return Items.Find( ( x ) => string.Equals( x.UID, uid, StringComparison.InvariantCultureIgnoreCase ) );
        }
    }
}
