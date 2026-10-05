using Cysharp.Threading.Tasks;
using Game.Utils;
using System;
using System.Threading;
using UnityEngine;

namespace Game.Core.Gameplay.World.Player
{
    public sealed class PlayerInputController
    {
        private Camera _camera;
        private CancellationTokenSource _cancellationTokenSource;

        private readonly PlayerConfig _config;
        private readonly PlayerMovementController _movementController;
        private readonly PlayerCameraController _cameraController;

        public PlayerInputController( PlayerConfig config, PlayerMovementController movementController, PlayerCameraController cameraController )
        {
            _config = config ?? throw new ArgumentNullException( nameof(config) );
            _movementController = movementController ?? throw new ArgumentNullException( nameof(movementController) );
            _cameraController = cameraController ?? throw new ArgumentNullException( nameof(cameraController) );
        }

        public void Initialize()
        {
            _camera = Camera.main;

            _cancellationTokenSource = new CancellationTokenSource();
            Tick( _cancellationTokenSource.Token ).Forget();
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }

        private async UniTask Tick( CancellationToken cancellationToken )
        {
            while ( !cancellationToken.IsCancellationRequested )
            {
                cancellationToken.ThrowIfCancellationRequested();

                HandleInput();
                HandleCameraInput();
                HandleCameraZoom();

                await UniTask.Yield( PlayerLoopTiming.Update, cancellationToken );
            }
        }

        private void HandleInput()
        {
            if ( !Input.GetMouseButtonDown( 0 ) ) return;

            Vector3 mousePosition = Input.mousePosition;
            if ( UIUtils.IsOverUI( mousePosition ) ) return;

            Ray ray = _camera.ScreenPointToRay( mousePosition );
            if ( !Physics.Raycast( ray, out RaycastHit hit, Mathf.Infinity, _config.GroundLayerMask ) )
            {
                return;
            }

            _movementController.MoveTo( hit.point );
        }

        private void HandleCameraInput()
        {
            // Например Tab переключает Follow / Free.
            if ( Input.GetKeyDown( KeyCode.Tab ) )
            {
                _cameraController.ToggleFollow();
            }

            Vector2 moveInput = Vector2.zero;

            if ( Input.GetKey( KeyCode.W ) )
                moveInput.y += 1f;

            if ( Input.GetKey( KeyCode.S ) )
                moveInput.y -= 1f;

            if ( Input.GetKey( KeyCode.D ) )
                moveInput.x += 1f;

            if ( Input.GetKey( KeyCode.A ) )
                moveInput.x -= 1f;

            _cameraController.SetMoveInput( moveInput );
        }

        private void HandleCameraZoom()
        {
            Vector3 mousePosition = Input.mousePosition;

            if ( UIUtils.IsOverUI( mousePosition ) )
                return;

            float zoomInput = Input.mouseScrollDelta.y;

            if ( Mathf.Approximately( zoomInput, 0f ) )
                return;

            _cameraController.SetZoomInput( zoomInput );
        }
    }
}
