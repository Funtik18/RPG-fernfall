using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game.Systems.CameraSystem
{
    public sealed class CameraFacingService
    {
        private CancellationTokenSource _cancellationTokenSource;
        private Camera _camera;
        
        private readonly List< Transform > _transforms = new();
        private readonly HashSet< Transform > _registeredTransforms = new();

        public void Initialize()
        {
            _cancellationTokenSource = new();
            Tick( _cancellationTokenSource.Token ).Forget();
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }

        public void Add( Transform transform )
        {
            if ( transform == null )
                return;

            if ( !_registeredTransforms.Add( transform ) )
                return;

            _transforms.Add( transform );
        }

        public void Remove( Transform transform )
        {
            if ( transform == null )
                return;

            if ( !_registeredTransforms.Remove( transform ) )
                return;

            _transforms.Remove( transform );
        }

        private async UniTask Tick( CancellationToken cancellationToken )
        {
            while ( !cancellationToken.IsCancellationRequested )
            {
                UpdateCamera();
                if ( _camera != null )
                {
                    UpdateTransforms();
                }

                await UniTask.Yield( PlayerLoopTiming.LastPostLateUpdate, cancellationToken );
            }
        }

        private void UpdateCamera()
        {
            if ( _camera != null )
                return;

            _camera = Camera.main;
        }

        private void UpdateTransforms()
        {
            Transform cameraTransform = _camera.transform;

            for ( int i = _transforms.Count - 1; i >= 0; i-- )
            {
                Transform transform = _transforms[ i ];
                if ( transform == null )
                {
                    _transforms.RemoveAt( i );
                    continue;
                }
                Vector3 direction = transform.position - cameraTransform.position;
                transform.rotation = Quaternion.LookRotation( direction, cameraTransform.up );
            }
        }
    }
}