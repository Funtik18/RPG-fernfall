using Game.Systems.StorageSystem;
using System;

namespace Game.Core.Systems.SheetSystem
{
    public sealed class InventoryItem
    {
        public event Action< InventoryItem > OnChanged;

        public string UID => Config.UID;

        public ItemConfig Config { get; }
        
        public InventoryItemData Data { get; set; }
        
        public int Quantity
        {
            get => _quantity;
            set
            {
                if ( Quantity != value )
                {
                    _quantity = value;
                    OnChanged?.Invoke( this );
                }
            }
        }
        private int _quantity;

        public InventoryItem( ItemConfig config, int quantity = 1 )
        {
            Config = config ?? throw new ArgumentNullException( nameof(config) );
            _quantity = quantity;
        }
        
        public void SetData( InventoryItemData data )
        {
            Data = data ?? throw new ArgumentNullException( nameof(data) );
        }
    }
}
