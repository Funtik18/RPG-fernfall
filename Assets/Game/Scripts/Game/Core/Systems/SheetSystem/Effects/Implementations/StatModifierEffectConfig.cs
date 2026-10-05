using System;
using UnityEngine;
using Value;

namespace Game.Core.Systems.SheetSystem
{
    [ CreateAssetMenu( fileName = "StatModifierEffect", menuName = "Game/Sheet/Effects/Stat Modifier" ) ]
    public sealed class StatModifierEffectConfig : CellEffectConfig
    {
        [ field: Header( "TODO RM" ) ]
        [ field: SerializeField ] public StatsType Stat { get; private set; } = StatsType.Strength;
        [ field: SerializeField ] public float ModifierValue { get; private set; } = -1f;

        public override Type GetEffectType() => typeof(StatModifierEffect);
    }
    
    public sealed class StatModifierEffect : Effect
    {
        private readonly StatModifierEffectConfig _config;

        private IStat _stat;
        private AddAttributeModifier _modifier;

        public override bool RemoveOnCellExit => _config.RemoveOnCellExit;

        public StatModifierEffect( IEffectSettings settings ) : base( settings )
        {
            _config = settings as StatModifierEffectConfig ?? throw new ArgumentException( $"Expected {nameof(StatModifierEffectConfig)}.", nameof(settings) );
        }

        public override void Apply( Sheet sheet )
        {
            _stat = GetStat( sheet, _config.Stat );
            _modifier = new AddAttributeModifier( _config.ModifierValue );
            _stat.AddModifier( _modifier );
        }

        public override void Remove( Sheet sheet )
        {
            _stat.RemoveModifier( _modifier );
            _stat = null;
            _modifier = null;
        }

        private static IStat GetStat( Sheet sheet, StatsType stat )
        {
            switch ( stat )
            {
                case StatsType.Strength:
                    return sheet.Stats.Strength;
                case StatsType.Dexterity:
                    return sheet.Stats.Dexterity;
                case StatsType.Luck:
                    return sheet.Stats.Luck;
                case StatsType.Defense:
                    return sheet.Stats.Defense;
                case StatsType.Resist:
                    return sheet.Stats.Resist;
                case StatsType.Craft:
                    return sheet.Stats.Craft;
                case StatsType.Avoid:
                    return sheet.Stats.Avoid;
                case StatsType.Hit:
                    return sheet.Stats.Hit;
                case StatsType.Critical:
                    return sheet.Stats.Critical;
                case StatsType.DefenseIgnore:
                    return sheet.Stats.DefenseIgnore;
                case StatsType.MovePoints:
                    return sheet.Stats.MovePoints;
                case StatsType.Fury:
                    return sheet.Stats.RagePoints;
                default:
                    throw new ArgumentOutOfRangeException( nameof(stat), stat, null );
            }
        }
    }
}
