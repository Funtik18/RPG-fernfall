using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Gameplay.TBS
{
    public sealed class UnitMovementController
    {
        private const float MoveSpeed = 4f;
        private const float JumpHeight = 1.25f;
        private const float MinJumpDuration = 0.2f;
        private const float RotationSpeed = 720f;
        private const float StopDistance = 0.05f;
        private const float StopAngle = 1f;
        private const float DirectionEpsilon = 0.0001f;
        private const float HeightEpsilon = 0.001f;

        private readonly UnitObject _view;
        private readonly UnitAnimatorController _animatorController;

        public UnitMovementController(
            UnitObject view,
            UnitAnimatorController animatorController
            )
        {
            _view = view ?? throw new ArgumentNullException( nameof(view) );
            _animatorController = animatorController ?? throw new ArgumentNullException( nameof(animatorController) );

            _view.LockRigidbodyToGrid();
        }

        public async UniTask Move( BattleCell start, IReadOnlyList< BattleCell > path )
        {
            if ( path == null || path.Count == 0 ) return;

            _view.LockRigidbodyToGrid();

            var current = start;
            foreach ( var cell in path )
            {
                await MoveTo( current, cell );

                current = cell;
            }
        }

        public async UniTask FaceTowards( Vector3 position )
        {
            if ( !TryGetLookRotation( position, out var targetRotation ) )
            {
                return;
            }

            while ( Quaternion.Angle( _view.transform.rotation, targetRotation ) > StopAngle )
            {
                SetRotation( Quaternion.RotateTowards(
                    _view.transform.rotation,
                    targetRotation,
                    RotationSpeed * Time.deltaTime
                ) );

                await UniTask.Yield();
            }

            SetRotation( targetRotation );
        }

        private UniTask MoveTo( BattleCell current, BattleCell next )
        {
            if ( next == null || next.View == null )
            {
                return UniTask.CompletedTask;
            }

            var position = next.View.transform.position;
            var transitionType = GetTransitionType( current, next );

            switch ( transitionType )
            {
                case GridCellTransitionType.Jump:
                    return JumpTo( position );

                case GridCellTransitionType.Teleport:
                    TeleportTo( position );
                    return UniTask.CompletedTask;

                case GridCellTransitionType.Climb:
                    return ClimbTo( position );

                default:
                    return MoveTo( position );
            }
        }

        private async UniTask MoveTo( Vector3 position )
        {
            _animatorController.SetSpeed( 1f );

            try
            {
                while ( Vector3.Distance( _view.transform.position, position ) > StopDistance )
                {
                    RotateTowards( position );
                    SetPosition( Vector3.MoveTowards( _view.transform.position, position, MoveSpeed * Time.deltaTime ) );

                    await UniTask.Yield();
                }
            }
            finally
            {
                _animatorController.SetSpeed( 0f );
            }

            SetPosition( position );
        }

        private async UniTask JumpTo( Vector3 position )
        {
            var start = _view.transform.position;
            var duration = GetMoveDuration( start, position );
            var animation = _animatorController.PlayJump( duration );

            await MoveAlongArc( start, position, duration );
            await animation;
        }

        private async UniTask ClimbTo( Vector3 position )
        {
            var start = _view.transform.position;
            var duration = GetMoveDuration( start, position );
            var animation = _animatorController.PlayClimb( duration );

            await MoveAlongArc( start, position, duration );
            await animation;
        }

        private async UniTask MoveAlongArc( Vector3 start, Vector3 position, float duration )
        {
            var elapsed = 0f;
            var jumpHeight = Mathf.Max( JumpHeight, Mathf.Abs( position.y - start.y ) + 0.25f );

            while ( elapsed < duration )
            {
                RotateTowards( position );

                var timeStep = Time.deltaTime > 0f ? Time.deltaTime : 0.016f;
                elapsed += timeStep;

                var t = Mathf.Clamp01( elapsed / duration );
                var current = Vector3.Lerp( start, position, t );
                current.y += Mathf.Sin( t * Mathf.PI ) * jumpHeight;

                SetPosition( current );

                await UniTask.Yield();
            }

            SetPosition( position );
        }

        private void TeleportTo( Vector3 position )
        {
            if ( TryGetLookRotation( position, out var targetRotation ) )
            {
                SetRotation( targetRotation );
            }

            SetPosition( position );
        }

        private GridCellTransitionType? GetTransitionType( BattleCell current, BattleCell next )
        {
            if ( current != null && current.TryGetConnection( next, out var connection ) && connection.IsGridLink )
            {
                return connection.TransitionType;
            }

            if ( HasHeightDifference( current, next ) )
            {
                return GridCellTransitionType.Jump;
            }

            return null;
        }

        private bool HasHeightDifference( BattleCell current, BattleCell next )
        {
            if ( current == null || next == null || current.View == null || next.View == null )
            {
                return false;
            }

            if ( current.View.Position.Y != next.View.Position.Y )
            {
                return true;
            }

            return Mathf.Abs( current.View.transform.position.y - next.View.transform.position.y ) > HeightEpsilon;
        }

        private float GetMoveDuration( Vector3 start, Vector3 position )
        {
            var distance = Vector3.Distance( start, position );
            return Mathf.Max( MinJumpDuration, distance / MoveSpeed );
        }

        private void RotateTowards( Vector3 position )
        {
            if ( !TryGetLookRotation( position, out var targetRotation ) )
            {
                return;
            }

            SetRotation( Quaternion.RotateTowards(
                _view.transform.rotation,
                targetRotation,
                RotationSpeed * Time.deltaTime
            ) );
        }

        private bool TryGetLookRotation( Vector3 position, out Quaternion rotation )
        {
            rotation = Quaternion.identity;

            var direction = position - _view.transform.position;
            direction.y = 0f;

            if ( direction.sqrMagnitude <= DirectionEpsilon )
            {
                return false;
            }

            rotation = Quaternion.LookRotation( direction, Vector3.up );
            return true;
        }

        private void SetPosition( Vector3 position )
        {
            if ( _view.Rigidbody != null )
            {
                _view.LockRigidbodyToGrid();
                _view.Rigidbody.position = position;
            }

            _view.transform.position = position;
        }

        private void SetRotation( Quaternion rotation )
        {
            if ( _view.Rigidbody != null )
            {
                _view.LockRigidbodyToGrid();
                _view.Rigidbody.rotation = rotation;
            }

            _view.transform.rotation = rotation;
        }
    }
}
