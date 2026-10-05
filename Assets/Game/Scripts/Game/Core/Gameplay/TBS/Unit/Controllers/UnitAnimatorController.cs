using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Game.Core.Gameplay.TBS
{
    public sealed class UnitAnimatorController
    {
        private const int BASE_LAYER_INDEX = 0;
        private const float ENTER_STATE_TIMEOUT = 0.5f;
        private const float STATE_COMPLETION_TIMEOUT = 5f;
        
        private static readonly int SPEED_FLOAT_HASH = Animator.StringToHash( "Speed" );
        private static readonly int ATTACK_TRIGGER_HASH = Animator.StringToHash( "Attack" );
        private static readonly int HIT_TRIGGER_HASH = Animator.StringToHash( "Hit" );
        private static readonly int DODGE_TRIGGER_HASH = Animator.StringToHash( "Dodge" );
        private static readonly int IS_DEATH_BOOL_HASH = Animator.StringToHash( "IsDeath" );
        private static readonly int IS_DEAD_BOOL_HASH = Animator.StringToHash( "IsDead" );
        // private static readonly int DEATH_STATE_HASH = Animator.StringToHash( "Death" );
        
        private readonly Vector3 _rootLocalPosition;
        private readonly Vector3 _rootLocalScale;
        private readonly Vector3 _rootLocalEulerAngles;

        private readonly Animator _animator;
        private readonly Transform _root;

        public UnitAnimatorController( UnitObject view )
        {
            _animator = view.Animator;
            _root = view.MeshRoot;
            _rootLocalPosition = _root.localPosition;
            _rootLocalScale = _root.localScale;
            _rootLocalEulerAngles = _root.localEulerAngles;
        }

        public UniTask PlayMove()
        {
            return UniTask.CompletedTask;
        }

        public async UniTask PlayJump( float duration )
        {
            if ( _root == null )
                return;

            ResetRoot();

            var takeoffDuration = Mathf.Min( 0.12f, duration * 0.25f );
            var landingDuration = Mathf.Min( 0.14f, duration * 0.30f );
            var airborneDuration = Mathf.Max( 0f, duration - takeoffDuration - landingDuration );
            var takeoffStepDuration = Mathf.Max( 0.01f, takeoffDuration * 0.5f );
            var landingStepDuration = Mathf.Max( 0.01f, landingDuration * 0.5f );

            var squashScale = new Vector3( _rootLocalScale.x * 1.08f, _rootLocalScale.y * 0.88f, _rootLocalScale.z * 1.08f );
            var stretchScale = new Vector3( _rootLocalScale.x * 0.94f, _rootLocalScale.y * 1.08f, _rootLocalScale.z * 0.94f );

            var sequence = DOTween.Sequence().SetTarget( _root );
            sequence.Append( _root.DOScale( squashScale, takeoffStepDuration ).SetEase( Ease.OutQuad ) );
            sequence.Join( _root.DOLocalMove( _rootLocalPosition + Vector3.down * 0.05f, takeoffStepDuration ).SetEase( Ease.OutQuad ) );
            sequence.Append( _root.DOScale( stretchScale, takeoffStepDuration ).SetEase( Ease.OutQuad ) );
            sequence.Join( _root.DOLocalMove( _rootLocalPosition + Vector3.up * 0.08f, takeoffStepDuration ).SetEase( Ease.OutQuad ) );

            if ( airborneDuration > 0f )
            {
                sequence.AppendInterval( airborneDuration );
            }

            sequence.Append( _root.DOScale( squashScale, landingStepDuration ).SetEase( Ease.OutQuad ) );
            sequence.Join( _root.DOLocalMove( _rootLocalPosition + Vector3.down * 0.04f, landingStepDuration ).SetEase( Ease.OutQuad ) );
            sequence.Append( _root.DOScale( _rootLocalScale, landingStepDuration ).SetEase( Ease.OutBack ) );
            sequence.Join( _root.DOLocalMove( _rootLocalPosition, landingStepDuration ).SetEase( Ease.OutBack ) );

            await sequence.AsyncWaitForCompletion();
        }

        public UniTask PlayClimb( float duration )
        {
            return UniTask.CompletedTask;
        }

        public async UniTask PlayAttack()
        {
            if ( !CanAnimate() ) return;
            
            _animator.SetTrigger( ATTACK_TRIGGER_HASH );
            await WaitForStateCompletion( ATTACK_TRIGGER_HASH );
        }

        public async UniTask PlayDamage()
        {
            if ( !CanAnimate() ) return;
            
            _animator.SetTrigger( HIT_TRIGGER_HASH );
            await WaitForStateCompletion( HIT_TRIGGER_HASH );
        }

        public async UniTask PlayDodge()
        {
            if ( !CanAnimate() ) return;

            _animator.SetTrigger( DODGE_TRIGGER_HASH );
            await WaitForStateCompletion( DODGE_TRIGGER_HASH );
        }

        public async UniTask PlayDeath()
        {
            if ( !CanAnimate() ) return;
            
            SetSpeed( 0f );
            TrySetDeathBool( true );
            // await WaitForStateCompletion( DEATH_STATE_HASH );
        }

        public void SetSpeed( float speed )
        {
            if ( !CanAnimate() ) return;
            
            _animator.SetFloat( SPEED_FLOAT_HASH, Mathf.Clamp01( speed ) );
        }
        
        private async UniTask WaitForStateCompletion( int stateHash )
        {
            var enterDeadline = Time.realtimeSinceStartup + ENTER_STATE_TIMEOUT;
            while ( CanAnimate() && Time.realtimeSinceStartup < enterDeadline )
            {
                if ( IsCurrentOrNextState( stateHash ) )
                {
                    break;
                }
                
                await UniTask.Yield();
            }
            
            if ( !CanAnimate() || !IsCurrentOrNextState( stateHash ) )
            {
                return;
            }
            
            var completionDeadline = Time.realtimeSinceStartup + STATE_COMPLETION_TIMEOUT;
            while ( CanAnimate() && Time.realtimeSinceStartup < completionDeadline )
            {
                if ( !IsCurrentOrNextState( stateHash ) )
                {
                    return;
                }
                
                var stateInfo = _animator.GetCurrentAnimatorStateInfo( BASE_LAYER_INDEX );
                if ( stateInfo.shortNameHash == stateHash && !_animator.IsInTransition( BASE_LAYER_INDEX ) && stateInfo.normalizedTime >= 1f )
                {
                    return;
                }
                
                await UniTask.Yield();
            }
        }
        
        private bool IsCurrentOrNextState( int stateHash )
        {
            var stateInfo = _animator.GetCurrentAnimatorStateInfo( BASE_LAYER_INDEX );
            if ( stateInfo.shortNameHash == stateHash )
            {
                return true;
            }
            
            if ( !_animator.IsInTransition( BASE_LAYER_INDEX ) )
            {
                return false;
            }
            
            var nextStateInfo = _animator.GetNextAnimatorStateInfo( BASE_LAYER_INDEX );
            return nextStateInfo.shortNameHash == stateHash;
        }
        
        private bool CanAnimate()
        {
            return _animator.isActiveAndEnabled && _animator.runtimeAnimatorController != null;
        }

        private bool TrySetDeathBool( bool isDeath )
        {
            return TrySetBool( IS_DEATH_BOOL_HASH, isDeath ) || TrySetBool( IS_DEAD_BOOL_HASH, isDeath );
        }

        private bool TrySetBool( int parameterHash, bool value )
        {
            if ( !HasParameter( parameterHash, AnimatorControllerParameterType.Bool ) )
            {
                return false;
            }

            _animator.SetBool( parameterHash, value );
            return true;
        }

        private bool HasParameter( int parameterHash, AnimatorControllerParameterType type )
        {
            foreach ( var parameter in _animator.parameters )
            {
                if ( parameter.type == type && parameter.nameHash == parameterHash )
                {
                    return true;
                }
            }

            return false;
        }

        private void ResetRoot()
        {
            _root.DOKill();

            _root.localPosition = _rootLocalPosition;
            _root.localScale = _rootLocalScale;
            _root.localEulerAngles = _rootLocalEulerAngles;
        }
    }
}
