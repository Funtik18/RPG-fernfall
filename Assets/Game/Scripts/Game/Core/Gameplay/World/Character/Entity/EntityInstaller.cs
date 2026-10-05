using UnityEngine;
using Zenject;

namespace Game.Core.Gameplay.World.Entity
{
    public sealed class EntityInstaller : MonoInstaller< EntityInstaller >
    {
        [ SerializeField ] private EntityObject _view;
        
        public override void InstallBindings()
        {
            Container.BindInstance( _view );
            
            Container.Bind< EntityController >().AsSingle().NonLazy();
        }
    }
}