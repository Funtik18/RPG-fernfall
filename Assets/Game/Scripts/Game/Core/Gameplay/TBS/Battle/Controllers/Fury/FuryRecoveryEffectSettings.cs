using Game.Core.Systems.SheetSystem;
using System;
using UnityEngine;
using Value;

namespace Game.Core.Gameplay.TBS
{
    [ Serializable ]
    public sealed class FuryExhaustedEffectSettings : EffectSettings
    {
        public override Type GetEffectType() => typeof(FuryExhaustedEffect);
    }

    public sealed class FuryExhaustedEffect : Effect
    {
        public override bool IsExpired => _remainingTurns <= 0;

        private IStat _statSpeed;
        private AttributeModifier _statSpeedModifier;
        private int _remainingTurns;

        private readonly FuryExhaustedEffectSettings _settings;

        public FuryExhaustedEffect( FuryExhaustedEffectSettings settings ) : base( settings )
        {
            _settings = settings ?? throw new ArgumentNullException( nameof(settings) );
        }

        public override void Apply( Sheet sheet )
        {
            _statSpeed = sheet.Stats.Speed;
            _statSpeedModifier = new AddAttributeModifier( -_settings.X );
            _statSpeed.AddModifier( _statSpeedModifier );

            Refresh( sheet );
        }

        public override void Refresh( Sheet sheet )
        {
            _remainingTurns = Mathf.Max( 0, _settings.Y );
        }

        public override void TickUnitCompleted( Sheet sheet )
        {
            if ( _remainingTurns <= 0 ) return;

            _remainingTurns--;
        }

        public override void Remove( Sheet sheet )
        {
            _statSpeed.RemoveModifier( _statSpeedModifier );
            _statSpeed = null;
            _statSpeedModifier = null;
        }
    }
}
