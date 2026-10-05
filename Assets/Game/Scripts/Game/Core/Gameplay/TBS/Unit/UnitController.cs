using Game.Core.Systems.SheetSystem;
using System;
using UnityEngine;

namespace Game.Core.Gameplay.TBS
{
    public sealed class UnitController
    {
        public UnitObject View { get; }
        public UnitMovementController Movement { get; }
        public UnitMotionController Motion { get; }
        public UnitHighlightController Highlighter { get; }
        public UnitGUIController GUI { get; }
        public UnitAnimatorController Animator { get; }
        public UnitEffectController Effects { get; }
        public UnitSkillController Skills { get; }
        
        public UnitConfig Config { get; private set; }
        public Sheet Sheet { get; private set; }
        
        public int RemainingMovePoints { get; private set; }
        public int RemainingAttacks { get; private set; }

        public bool HasMoved { get; private set; }
        public bool HasMovedDuringLastTurn { get; private set; }
        public bool IsRetreated { get; private set; }

        public bool CanMove => !HasMoved && RemainingMovePoints > 0;
        public bool CanAttack => RemainingAttacks > 0;
        
        public UnitController(
            UnitObject view,
            UnitMovementController movementController,
            UnitMotionController motionController,
            UnitHighlightController highlightController,
            UnitGUIController guiController,
            UnitAnimatorController animatorController,
            UnitEffectController effectController,
            UnitSkillController skillsController
            )
        {
            View = view ?? throw new ArgumentNullException( nameof(view) );
            Movement = movementController ?? throw new ArgumentNullException( nameof(movementController) );
            Motion = motionController ?? throw new ArgumentNullException( nameof(motionController) );
            Highlighter = highlightController ?? throw new ArgumentNullException( nameof(highlightController) );
            GUI = guiController ?? throw new ArgumentNullException( nameof(guiController) );
            Animator = animatorController ?? throw new ArgumentNullException( nameof(animatorController) );
            Effects = effectController ?? throw new ArgumentNullException( nameof(effectController) );
            Skills = skillsController ?? throw new ArgumentNullException( nameof(skillsController) );
            
            View.SetController( this );
            Effects.SetOwner( this );
            Skills.SetOwner( this );
        }

        public void SetConfig( UnitConfig config )
        {
            Config = config;
        }
        
        public void SetSheet( Sheet sheet )
        {
            Sheet = sheet;
            GUI.Initialize( sheet );
        }

        public void Dispose()
        {
            Skills.Dispose();
            Effects.Dispose();
            GUI.Dispose();
        }

        public void Death()
        {
            View.EnableCollider( false );
            GUI.Enable( false );
        }

        public void Retreat()
        {
            IsRetreated = true;
            View.EnableCollider( false );
            GUI.Enable( false );
        }
        
        public bool IsAlive() => Sheet.Stats.HealthPoints.Value > Sheet.Stats.HealthPoints.MinValue;

        public bool IsInBattle() => IsAlive() && !IsRetreated;
        
        public void Teleport( Vector3 position, Vector3 forward )
        {
            View.LockRigidbodyToGrid();

            var rotation = Quaternion.LookRotation( forward );

            if ( View.Rigidbody != null )
            {
                View.Rigidbody.position = position;
                View.Rigidbody.rotation = rotation;
            }

            View.transform.position = position;
            View.transform.rotation = rotation;
        }

        public void Restore()
        {
            RemainingMovePoints = GetMovePoints();
            RemainingAttacks = 1;
            HasMoved = false;
        }

        public int GetMovePoints() => (int)Sheet.Stats.MovePoints.TotalValue;

        public int GetAttackRange() => RemainingMovePoints + GetWeaponRange( Sheet.Equipment.Weapon );

        public int GetAttackRangeAfterMovement() => GetWeaponRange( Sheet.Equipment.Weapon );

        public int GetAttackRangeAfterMovement( Weapon weapon ) => GetWeaponRange( weapon );

        private int GetWeaponRange( Weapon weapon ) => Math.Max( 0, weapon?.Range ?? 0 );

        public void CompleteMove()
        {
            if ( HasMoved )
            {
                throw new InvalidOperationException( "Unit has already moved during this round." );
            }
            
            HasMoved = true;
            RemainingMovePoints = 0;
        }

        public void CompleteTurn()
        {
            HasMovedDuringLastTurn = HasMoved;
        }

        public void ClampRemainingMovePoints()
        {
            RemainingMovePoints = Math.Min( RemainingMovePoints, GetMovePoints() );
        }
        
        public bool CanSpendMovePoints( int amount )
        {
            return amount > 0 && RemainingMovePoints >= amount;
        }
        
        public void SpendMovePoints( int amount )
        {
            if ( !CanSpendMovePoints( amount ) )
            {
                throw new InvalidOperationException( $"Not enough move points. Required: {amount}, available: {RemainingMovePoints}." );
            }

            RemainingMovePoints -= amount;
        }

        public void SpendAttack()
        {
            if ( !CanAttack )
            {
                throw new InvalidOperationException( "Unit cannot attack anymore during this turn." );
            }

            RemainingAttacks--;
        }
    }
}
