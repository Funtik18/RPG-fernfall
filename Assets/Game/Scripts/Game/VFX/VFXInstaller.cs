using UnityEngine;
using Zenject;

namespace Game.VFX
{
    [ CreateAssetMenu( fileName = "VFXInstaller", menuName = "Installers/VFXInstaller" ) ]
    public sealed class VFXInstaller : ScriptableObjectInstaller< VFXInstaller >
    {
        [ SerializeField ] private VFXSettings _settings;

        public override void InstallBindings()
        {
            Container
                .BindFactory< VFXFloatingTextObject, VFXFloatingTextObject.Factory >()
                .FromMonoPoolableMemoryPool( ( x ) => x.WithInitialSize( 1 )
                    .FromComponentInNewPrefab( _settings.FloatingTextPrefab ) );
            Container.Bind< VFXFloatingTextObjectService >().AsSingle().Lazy();
        }
    }
}