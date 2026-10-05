using UnityEngine;
using Zenject;

namespace Game.Managers.SceneManager
{
    [ CreateAssetMenu( fileName = "SceneManagerInstaller", menuName = "Installers/SceneManagerInstaller" ) ]
    public sealed class SceneManagerInstaller : ScriptableObjectInstaller< SceneManagerInstaller >
    {
        [ SerializeField ] private SceneSettings _settings;
        
        public override void InstallBindings()
        {
            Container.BindInstance( _settings );
            
            Container.Bind< SceneLoader >().AsSingle().Lazy();
            Container.Bind< SceneManager >().AsSingle().Lazy();
        }
    }
}