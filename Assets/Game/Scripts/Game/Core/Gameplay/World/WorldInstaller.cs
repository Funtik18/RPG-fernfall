using UnityEngine;
using Zenject;

namespace Game.Core.Gameplay.World
{
    public sealed class WorldInstaller : MonoInstaller< WorldInstaller >
    {
        [ SerializeField ] private WorldObject _view;
        
        public override void InstallBindings()
        {
            Container.BindInstance( _view );
            
            Container.Bind< WorldController >().AsSingle().NonLazy();
        }
    }
}