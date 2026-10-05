using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace Game.Core.Gameplay.World.Player
{
    public sealed class PlayerCameraController
    {
        private bool _isFollowingPlayer = true;
        
        private Camera _camera;
        private CancellationTokenSource _cancellationTokenSource;
        private Vector3 _focusPosition;
        private Vector3 _freeFocusPosition;
        private Vector3 _focusVelocity;
        private Vector2 _moveInput;

        private float _currentDistance;
        private float _targetDistance;
        private float _zoomVelocity;

        private readonly PlayerConfig _config;
        private readonly PlayerObject _view;

        public PlayerCameraController( PlayerConfig config, PlayerObject view )
        {
            _config = config ?? throw new ArgumentNullException( nameof(config) );
            _view = view ?? throw new ArgumentNullException( nameof(view) );
        }

        public void Initialize()
        {
            _camera = Camera.main;

            _focusPosition = _view.Root.position;
            _freeFocusPosition = _focusPosition;
            _currentDistance = _config.CameraSettings.Distance;
            _targetDistance = _currentDistance;

            ApplyCameraPositionInstant();

            _cancellationTokenSource = new CancellationTokenSource();
            Tick( _cancellationTokenSource.Token ).Forget();
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }

        public void SetMoveInput( Vector2 input )
        {
            _moveInput = Vector2.ClampMagnitude( input, 1f );
        }

        public void SetZoomInput( float input )
        {
            if ( Mathf.Approximately( input, 0f ) )
                return;

            // Колесо вверх -> камера ближе.
            _targetDistance -= input * _config.CameraSettings.ZoomSpeed;

            _targetDistance = Mathf.Clamp( _targetDistance, _config.CameraSettings.MinDistance, _config.CameraSettings.MaxDistance );
        }

        public void ToggleFollow()
        {
            if ( _isFollowingPlayer )
            {
                Detach();
            }
            else
            {
                Attach();
            }
        }

        public void Attach()
        {
            _isFollowingPlayer = true;
            _moveInput = Vector2.zero;
        }

        public void Detach()
        {
            _isFollowingPlayer = false;
            _freeFocusPosition = _focusPosition;
        }

        private async UniTask Tick( CancellationToken cancellationToken )
        {
            while ( !cancellationToken.IsCancellationRequested )
            {
                cancellationToken.ThrowIfCancellationRequested();

                UpdateCamera();

                await UniTask.Yield( PlayerLoopTiming.LastPostLateUpdate, cancellationToken );
            }
        }

        private void UpdateCamera()
        {
            if ( !_isFollowingPlayer )
            {
                UpdateFreeMovement();
            }

            UpdateZoom();

            _focusPosition = Vector3.SmoothDamp( _focusPosition, _isFollowingPlayer ? _view.Root.position : _freeFocusPosition, ref _focusVelocity, _config.CameraSettings.FollowSmoothTime );

            Quaternion rotation = GetCameraRotation();
            Vector3 cameraPosition = _focusPosition - rotation * Vector3.forward * _currentDistance;

            _camera.transform.SetPositionAndRotation( cameraPosition, rotation );
        }

        private void UpdateZoom()
        {
            _currentDistance = Mathf.SmoothDamp( _currentDistance, _targetDistance, ref _zoomVelocity, _config.CameraSettings.ZoomSmoothTime );
        }

        private void UpdateFreeMovement()
        {
            if ( _moveInput.sqrMagnitude <= 0.001f )
                return;

            Vector3 forward = _camera.transform.forward;
            Vector3 right = _camera.transform.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            Vector3 direction = forward * _moveInput.y + right * _moveInput.x;
            if ( direction.sqrMagnitude > 1f )
            {
                direction.Normalize();
            }

            _freeFocusPosition += direction * _config.CameraSettings.FreeMoveSpeed * Time.deltaTime;
        }

        private Quaternion GetCameraRotation() => Quaternion.Euler( _config.CameraSettings.Pitch, _config.CameraSettings.Yaw, 0f );

        private void ApplyCameraPositionInstant()
        {
            Quaternion rotation = GetCameraRotation();
            Vector3 position = _focusPosition - rotation * Vector3.forward * _currentDistance;

            _camera.transform.SetPositionAndRotation( position, rotation );
        }
    }
}