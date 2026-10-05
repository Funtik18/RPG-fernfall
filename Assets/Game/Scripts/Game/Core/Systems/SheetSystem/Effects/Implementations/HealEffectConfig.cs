using System;
using UnityEngine;

namespace Game.Core.Systems.SheetSystem
{
    [ CreateAssetMenu( fileName = nameof(HealEffectConfig), menuName = "Game/Sheet/Effects/" + nameof(HealEffectConfig) ) ]
    public sealed class HealEffectConfig : EffectConfig
    {
        public override Type GetEffectType() => typeof(HealEffect);
    }

    public sealed class HealEffect : Effect
    {
        public override bool IsExpired => true;

        private readonly HealEffectConfig _config;

        public HealEffect( IEffectSettings settings ) : base( settings )
        {
            _config = settings as HealEffectConfig ?? throw new ArgumentException( $"Expected {nameof(HealEffectConfig)}.", nameof(settings) );
        }

        public override void Apply( Sheet sheet )
        {
            if ( sheet.IsDead ) return;
            if ( _config.X <= 0 ) return;

            sheet.Stats.HealthPoints.Value += _config.X;
        }
    }
}
