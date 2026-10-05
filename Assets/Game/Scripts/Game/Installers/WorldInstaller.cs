using Game.Core.Gameplay;
using Game.Core.Systems.CommandSystem;
using Game.Core.Systems.FeatureSystem;
using Game.GamePipeline;
using UnityEngine;
using Zenject;

namespace Game.Installers
{
    public sealed class WorldInstaller : MonoInstaller< WorldInstaller >
    {
        [ SerializeField ] private WorldGameplayConfig worldGameplayConfig;

        public override void InstallBindings()
        {
            Debug.Log( "[GamePipeline] GameplayInstaller" );

            Container.BindInstance( worldGameplayConfig );

            CommandSystemInstaller.Install( Container );
            FeatureSystemInstaller.Install( Container );
            
            Container.BindInterfacesAndSelfTo< BootstrapWorld >().AsSingle().NonLazy();
        }
    }
}
