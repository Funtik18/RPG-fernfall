using Zenject;

namespace Game.Core.Systems.SheetSystem
{
    public sealed class EffectFactory
    {
        private readonly DiContainer _diContainer;
        
        public EffectFactory( DiContainer diContainer )
        {
            _diContainer = diContainer ?? throw new System.ArgumentNullException( nameof(diContainer) );
        }

        public Effect Create( IEffectSettings settings )
        {
            return (Effect)_diContainer.Instantiate( settings.GetEffectType(), new object[] { settings } );
        }
    }
}
