using Game.Core.Systems.SheetSystem;
using System;
using System.Collections.Generic;

namespace Game.Core.Gameplay.TBS
{
    public sealed class UnitEffectController
    {
        private readonly List< Effect > _effects = new();
        
        private UnitController _owner;

        private readonly EffectFactory _effectFactory;
        
        public UnitEffectController( EffectFactory effectFactory )
        {
            _effectFactory = effectFactory ?? throw new ArgumentNullException( nameof(effectFactory) );
        }
        
        public void SetOwner( UnitController owner )
        {
            _owner = owner ?? throw new ArgumentNullException( nameof(owner) );
        }
        
        public void Dispose()
        {
            if ( _owner?.Sheet == null )
            {
                _effects.Clear();
                return;
            }

            for ( int i = _effects.Count - 1; i >= 0; i-- )
            {
                RemoveAt( i );
            }
        }

        public Effect Apply( IEffectSettings settings )
        {
            if ( settings == null ) return null;

            var effect = GetEffect( settings );
            if ( effect != null )
            {
                effect.Refresh( _owner.Sheet );
                RemoveExpiredEffects();
                return effect;
            }

            effect = _effectFactory.Create( settings );
            _effects.Add( effect );

            effect.Apply( _owner.Sheet );
            RemoveExpiredEffects();

            return effect;
        }

        public void TickRoundStarted()
        {
            for ( int i = _effects.Count - 1; i >= 0; i-- )
            {
                var effect = _effects[ i ];
                effect.TickRoundStarted( _owner.Sheet );

                if ( effect.IsExpired )
                {
                    RemoveAt( i );
                }
            }
        }

        public void TickUnitCompleted()
        {
            for ( int i = _effects.Count - 1; i >= 0; i-- )
            {
                var effect = _effects[ i ];
                effect.TickUnitCompleted( _owner.Sheet );

                if ( effect.IsExpired )
                {
                    RemoveAt( i );
                }
            }
        }

        public Effect GetEffect( IEffectSettings settings )
        {
            for ( int i = 0; i < _effects.Count; i++ )
            {
                var effect = _effects[ i ];
                if ( effect.Settings == settings )
                {
                    return effect;
                }
            }

            return null;
        }

        public void Remove( Effect effect )
        {
            if ( effect == null ) return;

            for ( int i = _effects.Count - 1; i >= 0; i-- )
            {
                if ( _effects[ i ] == effect )
                {
                    RemoveAt( i );
                    return;
                }
            }
        }
        
        public void Remove( IEffectSettings settings )
        {
            if ( settings == null ) return;

            for ( int i = _effects.Count - 1; i >= 0; i-- )
            {
                if ( _effects[ i ].Settings == settings )
                {
                    RemoveAt( i );
                    return;
                }
            }
        }

        public void RemoveCellEffects()
        {
            for ( int i = _effects.Count - 1; i >= 0; i-- )
            {
                if ( _effects[ i ].RemoveOnCellExit )
                {
                    RemoveAt( i );
                }
            }
        }
        
        private void RemoveExpiredEffects()
        {
            for ( int i = _effects.Count - 1; i >= 0; i-- )
            {
                if ( _effects[ i ].IsExpired )
                {
                    RemoveAt( i );
                }
            }
        }

        private void RemoveAt( int index )
        {
            var effect = _effects[ index ];
            effect.Remove( _owner.Sheet );
            _effects.RemoveAt( index );
        }
    }
}
