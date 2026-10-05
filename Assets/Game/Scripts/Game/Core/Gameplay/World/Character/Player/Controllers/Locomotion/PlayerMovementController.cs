using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace Game.Core.Gameplay.World.Player
{
    public sealed class PlayerMovementController
    {
        private CancellationTokenSource _cancellationTokenSource;
        private Rigidbody _rigidbody;
        private Vector3 _targetPosition;
        private bool _hasTarget;

        private readonly PlayerConfig _config;
        private readonly PlayerObject _view;

        public PlayerMovementController( PlayerConfig config, PlayerObject view )
        {
            _config = config ?? throw new ArgumentNullException( nameof(config) );
            _view = view ?? throw new ArgumentNullException( nameof(view) );
        }

        public void Initialize()
        {
            _rigidbody = _view.Rigidbody;

            _cancellationTokenSource = new CancellationTokenSource();

            Tick( _cancellationTokenSource.Token ).Forget();
        }
        
        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }

        public void MoveTo( Vector3 position )
        {
            _targetPosition = position;
            _hasTarget = true;
        }

        public void Stop()
        {
            _hasTarget = false;
            _rigidbody.linearVelocity = Vector3.zero;
        }

        private async UniTask Tick( CancellationToken cancellationToken )
        {
            while ( !cancellationToken.IsCancellationRequested )
            {
                UpdateMovement();

                await UniTask.Yield( PlayerLoopTiming.FixedUpdate, cancellationToken );
            }
        }

        private void UpdateMovement()
        {
            if ( !_hasTarget )
                return;

            Vector3 currentPosition = _rigidbody.position;
            Vector3 direction = _targetPosition - currentPosition;

            // Двигаемся только по земле.
            direction.y = 0f;

            float distance = direction.magnitude;

            if ( distance <= _config.MovementSettings.StopDistance )
            {
                _hasTarget = false;
                return;
            }

            Vector3 normalizedDirection = direction / distance;

            Move( currentPosition, normalizedDirection, distance );
            Rotate( normalizedDirection );
        }

        private void Move( Vector3 currentPosition, Vector3 direction, float distance )
        {
            float moveDistance = _config.MovementSettings.MoveSpeed * Time.fixedDeltaTime;

            // Не перелетаем через target.
            moveDistance = Mathf.Min( moveDistance, distance );
            Vector3 nextPosition = currentPosition + direction * moveDistance;
            _rigidbody.MovePosition( nextPosition );
        }

        private void Rotate( Vector3 direction )
        {
            if ( direction.sqrMagnitude <= 0.0001f )
                return;

            Quaternion targetRotation = Quaternion.LookRotation( direction, Vector3.up );
            Quaternion nextRotation = Quaternion.Slerp( _rigidbody.rotation, targetRotation, _config.MovementSettings.RotationSpeed * Time.fixedDeltaTime );

            _rigidbody.MoveRotation( nextRotation );
        }
    }
}