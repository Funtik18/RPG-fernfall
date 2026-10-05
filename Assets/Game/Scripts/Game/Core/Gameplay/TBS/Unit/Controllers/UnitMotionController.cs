using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace Game.Core.Gameplay.TBS
{
    public sealed class UnitMotionController
    {
        private const float FlySpeed = 7f;
        private const float MinFlyDuration = 0.18f;

        private readonly UnitObject _view;

        public UnitMotionController( UnitObject view )
        {
            _view = view ?? throw new ArgumentNullException( nameof(view) );

            _view.LockRigidbodyToGrid();
        }

        public async UniTask FlyBack( BattleCell start, BattleCell destination, int distance )
        {
            if ( destination?.View == null ) return;
            if ( distance <= 0 ) return;

            _view.LockRigidbodyToGrid();

            var from = start?.View == null ? _view.transform.position : start.View.transform.position;
            var to = destination.View.transform.position;
            var duration = GetFlyDuration( from, to );

            await FlyStraight( from, to, duration );

            SetPosition( to );
        }

        private async UniTask FlyStraight( Vector3 start, Vector3 destination, float duration )
        {
            var elapsed = 0f;

            while ( elapsed < duration )
            {
                var timeStep = Time.deltaTime > 0f ? Time.deltaTime : 0.016f;
                elapsed += timeStep;

                var t = Mathf.Clamp01( elapsed / duration );
                var easedT = 1f - Mathf.Pow( 1f - t, 2f );

                SetPosition( Vector3.Lerp( start, destination, easedT ) );

                await UniTask.Yield();
            }
        }

        private float GetFlyDuration( Vector3 start, Vector3 destination )
        {
            var distance = Vector3.Distance( start, destination );
            return Mathf.Max( MinFlyDuration, distance / FlySpeed );
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
    }
}
