using R3;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Gameplay.World.Player
{
    public sealed class PlayerTriggerController
    {
        public bool IsTriggerEnterEnabled { get; private set; }
        
        private InteractableTriggerObject _lastEnterInteractable;
        private readonly CompositeDisposable _disposables = new();
        
        private readonly PlayerConfig _config;
        private readonly PlayerAvatar _avatar;
        
        public PlayerTriggerController(
            PlayerConfig config,
            PlayerAvatar avatar
            )
        {
            _config = config ?? throw new ArgumentNullException( nameof(config) );
            _avatar = avatar ?? throw new ArgumentNullException( nameof(avatar) );
        }
        
        public void Initialize()
        {
            _avatar.TriggerCollider.Enter
                .Where( IsInteractableLayer )
                .Select( GetInteractableTrigger )
                .Where( interactable => interactable != null )
                .Subscribe( InteractableEnteredHandler )
                .AddTo( _disposables );
            
            _avatar.TriggerCollider.Exit
                .Where( IsInteractableLayer )
                .Select( GetInteractableTrigger )
                .Where( interactable => interactable != null )
                .Subscribe( InteractableExitedHandler )
                .AddTo( _disposables );
        }
        
        public void Dispose()
        {
            _disposables.Dispose();
        }

        public void EnableTriggerEnter( bool trigger )
        {
            IsTriggerEnterEnabled = trigger;
        }

        public void TryEnterLastInteractable()
        {
            _lastEnterInteractable?.OnTriggerEnter();
            _lastEnterInteractable = null;
        }

        private bool IsInteractableLayer( Collider collider ) => collider.gameObject.layer == _config.InteractableLayerMask;

        private InteractableTriggerObject GetInteractableTrigger( Collider collider ) => collider.GetComponentInParent< InteractableTriggerObject >();

        private void InteractableEnteredHandler( InteractableTriggerObject interactable )
        {
            if ( !IsTriggerEnterEnabled )
            {
                _lastEnterInteractable = interactable;
                return;
            }
            interactable.OnTriggerEnter();
        }
        
        private void InteractableExitedHandler( InteractableTriggerObject interactable )
        {
            if ( _lastEnterInteractable != null && _lastEnterInteractable == interactable )
            {
                _lastEnterInteractable = null;
            }
            interactable.OnTriggerExit();
        }
    }
}