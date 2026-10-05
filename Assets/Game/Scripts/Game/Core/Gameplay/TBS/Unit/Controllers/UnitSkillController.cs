using Game.Core.Systems.SheetSystem;
using System;
using System.Collections.Generic;

namespace Game.Core.Gameplay.TBS
{
    public sealed class UnitSkillController
    {
        private readonly List< Skill > _skills = new();
        
        private UnitController _owner;

        private readonly SkillFactory _skillFactory;

        public IReadOnlyList< Skill > Skills => _skills;
        
        public UnitSkillController( SkillFactory skillFactory )
        {
            _skillFactory = skillFactory ?? throw new ArgumentNullException( nameof(skillFactory) );
        }
        
        public void SetOwner( UnitController owner )
        {
            _owner = owner ?? throw new ArgumentNullException( nameof(owner) );
        }

        public void ApplyInitialSkills()
        {
            if ( _owner?.Sheet == null ) return;

            var settings = _owner.Sheet.Class?.Skills;
            if ( settings == null ) return;

            foreach ( var config in settings.Skills )
            {
                Apply( config.Skill );
            }
        }

        public Skill Apply( SkillConfig config )
        {
            if ( config == null ) return null;

            var skill = GetSkill( config );
            if ( skill != null )
            {
                return skill;
            }

            skill = _skillFactory.Create( config );
            _skills.Add( skill );

            skill.Apply( _owner.Sheet );

            return skill;
        }

        public void Refresh()
        {
            for ( int i = 0; i < _skills.Count; i++ )
            {
                _skills[ i ].Refresh( _owner.Sheet );
            }
        }

        public void OnBeforeAttack( UnitController target )
        {
            for ( int i = 0; i < _skills.Count; i++ )
            {
                _skills[ i ].OnBeforeAttack( _owner.Sheet, target.Sheet );
            }
        }

        public void OnAfterAttack( UnitController target )
        {
            for ( int i = 0; i < _skills.Count; i++ )
            {
                _skills[ i ].OnAfterAttack( _owner.Sheet, target.Sheet );
            }
        }

        public void OnBeforeHit( UnitController target, bool targetCanCounterattack, bool attackedFromDistance )
        {
            for ( int i = 0; i < _skills.Count; i++ )
            {
                _skills[ i ].OnBeforeHit( _owner.Sheet, target.Sheet, targetCanCounterattack, attackedFromDistance );
            }
        }

        public void OnAfterHit( UnitController target, bool isHit )
        {
            for ( int i = 0; i < _skills.Count; i++ )
            {
                _skills[ i ].OnAfterHit( _owner.Sheet, target.Sheet, isHit );
            }
        }

        public void Dispose()
        {
            if ( _owner?.Sheet == null )
            {
                _skills.Clear();
                return;
            }

            for ( int i = _skills.Count - 1; i >= 0; i-- )
            {
                _skills[ i ].Dispose( _owner.Sheet );
            }

            _skills.Clear();
        }

        public Skill GetSkill( SkillConfig config )
        {
            for ( int i = 0; i < _skills.Count; i++ )
            {
                var skill = _skills[ i ];
                if ( skill.Config == config )
                {
                    return skill;
                }
            }

            return null;
        }

        public bool Contains< TSkill >() where TSkill : Skill
        {
            for ( int i = 0; i < _skills.Count; i++ )
            {
                if ( _skills[ i ] is TSkill )
                {
                    return true;
                }
            }

            return false;
        }
    }
}
