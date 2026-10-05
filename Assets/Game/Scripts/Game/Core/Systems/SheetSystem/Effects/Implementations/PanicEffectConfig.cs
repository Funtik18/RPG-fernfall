using System;
using UnityEngine;
using Value;

namespace Game.Core.Systems.SheetSystem
{
    [ CreateAssetMenu( fileName = nameof(PanicEffectConfig), menuName = "Game/Sheet/Effects/" + nameof(PanicEffectConfig) ) ]
    public sealed class PanicEffectConfig : EffectConfig
    {
        public override Type GetEffectType() => typeof(PanicEffect);
    }

    public sealed class PanicEffect : Effect
    {
        public override bool IsExpired => _remainingTurns <= 0;

        private IStat _hit;
        private AttributeModifier _hitModifier;
        private IStat _avoid;
        private AttributeModifier _avoidModifier;
        private int _remainingTurns;

        private readonly PanicEffectConfig _config;
        
        public PanicEffect( IEffectSettings settings ) : base( settings )
        {
            _config = settings as PanicEffectConfig ?? throw new ArgumentException( $"Expected {nameof(PanicEffectConfig)}.", nameof(settings) );
        }

        public override void Apply( Sheet sheet )
        {
            _hit = sheet.Stats.Hit;
            _hitModifier = new AddAttributeModifier( -_config.X );
            _hit.AddModifier( _hitModifier );

            _avoid = sheet.Stats.Avoid;
            _avoidModifier = new AddAttributeModifier( -_config.Y );
            _avoid.AddModifier( _avoidModifier );

            Refresh( sheet );
        }

        public override void Refresh( Sheet sheet )
        {
            _remainingTurns = Math.Max( 0, _config.Z );
        }

        public override void TickRoundStarted( Sheet sheet )
        {
            if ( _remainingTurns <= 0 ) return;

            _remainingTurns--;
        }

        public override void Remove( Sheet sheet )
        {
            _hit.RemoveModifier( _hitModifier );
            _hit = null;
            _hitModifier = null;

            _avoid.RemoveModifier( _avoidModifier );
            _avoid = null;
            _avoidModifier = null;
        }
    }
}
