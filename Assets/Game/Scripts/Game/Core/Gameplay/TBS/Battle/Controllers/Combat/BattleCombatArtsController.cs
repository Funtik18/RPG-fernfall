using Game.Core.Systems.SheetSystem;
using System;
using System.Collections.Generic;

namespace Game.Core.Gameplay.TBS
{
    public sealed class BattleCombatArtsController
    {
        private readonly Dictionary< UnitController, Dictionary< CombatArtConfig, int > > _combatArtUses = new();
        
        private readonly CombatArtFactory _combatArtFactory;

        public BattleCombatArtsController( CombatArtFactory combatArtFactory )
        {
            _combatArtFactory = combatArtFactory ?? throw new ArgumentNullException( nameof(combatArtFactory) );
        }

        public void Initialize()
        {
            _combatArtUses.Clear();
        }

        public void Dispose()
        {
            _combatArtUses.Clear();
        }

        public CombatArt Create( UnitController attacker, CombatArtConfig config )
        {
            if ( !CanUseCombatArt( attacker, config ) ) return null;

            return _combatArtFactory.Create( config );
        }

        public bool CanUseCombatArt( UnitController attacker, CombatArtConfig config )
        {
            return CanUseCombatArt( attacker, attacker?.Sheet?.Equipment?.Weapon, config );
        }

        public bool CanUseCombatArt( UnitController attacker, Weapon weapon, CombatArtConfig config )
        {
            if ( !weapon.Config.Family.CombatArts.Contains( config ) ) return false;

            return GetCombatArtUsesRemaining( attacker, config ) > 0;
        }

        public int GetCombatArtUsesRemaining( UnitController attacker, CombatArtConfig config )
        {
            return Math.Max( 0, config.Uses - GetCombatArtUsesCount( attacker, config ) );
        }

        public bool TrySpendUse( UnitController attacker, CombatArtConfig config )
        {
            if ( !CanUseCombatArt( attacker, config ) ) return false;

            if ( !_combatArtUses.TryGetValue( attacker, out var unitUses ) )
            {
                unitUses = new Dictionary< CombatArtConfig, int >();
                _combatArtUses.Add( attacker, unitUses );
            }

            unitUses[ config ] = GetCombatArtUsesCount( attacker, config ) + 1;
            return true;
        }

        private int GetCombatArtUsesCount( UnitController attacker, CombatArtConfig config )
        {
            if ( !_combatArtUses.TryGetValue( attacker, out var unitUses ) ) return 0;

            return unitUses.TryGetValue( config, out var uses ) ? uses : 0;
        }
    }
}
