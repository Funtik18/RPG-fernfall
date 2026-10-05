using System;
using Zenject;

namespace Game.Core.Systems.SheetSystem
{
    public sealed class CombatArtFactory
    {
        private readonly DiContainer _diContainer;

        public CombatArtFactory( DiContainer diContainer )
        {
            _diContainer = diContainer ?? throw new ArgumentNullException( nameof(diContainer) );
        }

        public CombatArt Create( CombatArtConfig config )
        {
            if ( config == null ) return null;

            return (CombatArt)_diContainer.Instantiate( config.GetComboArtType(), new object[] { config } );
        }
    }
}