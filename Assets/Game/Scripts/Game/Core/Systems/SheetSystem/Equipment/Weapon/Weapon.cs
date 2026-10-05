using Game.Systems.StorageSystem;
using System;

namespace Game.Core.Systems.SheetSystem
{
    public sealed class Weapon : IMemento
    {
        public event Action< Weapon > OnChanged;

        public WeaponItemConfig Config { get; }
        public WeaponItemData Data { get; private set; }
        public bool IsBaseWeapon { get; }
        
        public int Might => Config.Might;
        public float Hit => Config.Hit;
        public float Critical => Config.Critical;
        public int Avoid => Config.Avoid;
        public int Range => Config.Range;
        
        public int Uses { get; private set; }
        public int SpecialUses { get; private set; }
        public bool CanUse => IsBaseWeapon || ( Config.IsInfinity ? SpecialUses > 0 : Uses > 0 );
        public bool IsBroken => !IsBaseWeapon && !Config.IsInfinity && Uses <= 0;
     
        public Weapon( WeaponItemConfig config, bool isBaseWeapon = false )
        {
            Config = config ?? throw new ArgumentNullException( nameof(config) );
            IsBaseWeapon = isBaseWeapon;

            Uses = config.Uses;
            ResetSpecialUses();
        }

        public void SetData( WeaponItemData data )
        {
            Data = data ?? throw new ArgumentNullException( nameof(data) );
            RestoreCommit();
        }

        public bool SpendUse()
        {
            if ( IsBaseWeapon ) return true;

            if ( Config.IsInfinity )
            {
                if ( SpecialUses <= 0 ) return false;

                SpecialUses--;
                OnChanged?.Invoke( this );

                return true;
            }

            if ( Uses <= 0 ) return false;

            Uses--;
            Commit();
            OnChanged?.Invoke( this );

            return true;
        }

        public void ResetSpecialUses()
        {
            SpecialUses = Config.SpecialUses;
        }

        public void Commit()
        {
            if ( Data == null )
            {
                Data = new();
            }

            Data.Uses = Uses;
        }

        public void RestoreCommit()
        {
            if ( Data == null ) return;

            Uses = Data.Uses;
        }
    }
}
