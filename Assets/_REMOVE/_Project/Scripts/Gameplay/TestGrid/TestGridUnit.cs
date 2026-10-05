using System.Collections.Generic;
using System.Threading.Tasks;
using TurnBasedStrategyFramework.Common.Cells;
using TurnBasedStrategyFramework.Unity.Units;
using UnityEngine;

namespace Fire.Gameplay.TestGrid
{
    // Vertical-slice test unit: TBSF drives transform via Unit.MovementAnimation (virtual),
    // it has no opinion on animation, so we override it to bridge into an Animator bool.
    public class TestGridUnit : Unit
    {
        // TBSF's IUnit has no name concept at all (only an int UnitID) - this is a standalone
        // field for the combat HUD's selected-unit panel, not a general naming system. Known
        // future seam: this and ScrollRecordData.Name are two separate "a character's name"
        // concepts that will eventually need reconciling once units and Scroll records
        // represent the same characters - deliberately not unified now (see CLAUDE.md).
        public string DisplayName;

        [SerializeField] private Animator _animator;
        private static readonly int IsWalking = Animator.StringToHash("IsWalking");

public override async Task MovementAnimation(IEnumerable<ICell> path, ICell destination)
        {
            if (_animator != null) _animator.SetBool(IsWalking, true);

            _isMoving = true;
            var facingTask = FaceMovementDirectionWhileMoving();
            await base.MovementAnimation(path, destination);
            _isMoving = false;
            await facingTask;

            if (_animator != null) _animator.SetBool(IsWalking, false);
        }

        // TBSF only moves the transform, it never rotates it - so we face the direction
        // we're actually moving each frame. Not resetting rotation on arrival is what makes
        // the unit keep looking the way it last walked once it stops.
        private bool _isMoving;
        private async Task FaceMovementDirectionWhileMoving()
        {
            var lastPosition = transform.position;
            while (_isMoving)
            {
                await Awaitable.NextFrameAsync();
                var delta = transform.position - lastPosition;
                delta.y = 0f;
                if (delta.sqrMagnitude > 0.0001f)
                {
                    transform.rotation = Quaternion.LookRotation(delta.normalized);
                }
                lastPosition = transform.position;
            }
        }
    

private static readonly int AttackTrigger = Animator.StringToHash("Attack");
        private static readonly int HitTrigger = Animator.StringToHash("Hit");
        private static readonly int DeathTrigger = Animator.StringToHash("Death");

public override async Task MarkAsAttacking(Unit otherUnit)
        {
            // Face the target and stay facing it - same "don't reset rotation" pattern as movement.
            if (otherUnit != null)
            {
                var direction = otherUnit.transform.position - transform.position;
                direction.y = 0f;
                if (direction.sqrMagnitude > 0.0001f)
                {
                    transform.rotation = Quaternion.LookRotation(direction.normalized);
                }
            }

            if (_animator != null) _animator.SetTrigger(AttackTrigger);
            await base.MarkAsAttacking(otherUnit);
        }

        // Health is already reduced by the time this runs (ModifyHealth runs before
        // MarkAsAttacking/MarkAsDefending in AttackCommand) - skip the hit react on a
        // lethal hit so it doesn't fight the death animation on the same Animator.
        public override async Task MarkAsDefending(Unit otherUnit)
        {
            if (Health > 0 && _animator != null) _animator.SetTrigger(HitTrigger);
            await base.MarkAsDefending(otherUnit);
        }

        public override async Task MarkAsDestroyed()
        {
            if (_animator != null) _animator.SetTrigger(DeathTrigger);
            await base.MarkAsDestroyed();
        }

        // Deliberately skips base.OnDestroyed() (which would Destroy(gameObject) immediately) -
        // the corpse stays where it fell for this test; only stops it from being clickable.
        public override void OnDestroyed(TurnBasedStrategyFramework.Common.Controllers.IGridController gridController)
        {
            var unitCollider = GetComponent<Collider>();
            if (unitCollider != null) unitCollider.enabled = false;
        }
}
}
