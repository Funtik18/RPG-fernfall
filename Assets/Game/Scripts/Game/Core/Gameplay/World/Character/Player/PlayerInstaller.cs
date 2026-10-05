using UnityEngine;
using Zenject;

namespace Game.Core.Gameplay.World.Player
{
    public sealed class PlayerInstaller : MonoInstaller< PlayerInstaller >
    {
        [ SerializeField ] private PlayerConfig _config;
        [ SerializeField ] private PlayerObject _view;
        
        public override void InstallBindings()
        {
            Container.BindInstance( _config );
            Container.BindInstance( _view );
            Container.BindInstance( _view.Avatar );

            Container.Bind< PlayerInputController >().AsSingle().Lazy();
            Container.Bind< PlayerMovementController >().AsSingle().Lazy();
            Container.Bind< PlayerCameraController >().AsSingle().Lazy();
            Container.Bind< PlayerTriggerController >().AsSingle().Lazy();

            Container.Bind< PlayerContext >().AsSingle().Lazy();
            Container.Bind< PlayerController >().AsSingle().NonLazy();
        }
    }
}