using Game.Core.Gameplay;
using Game.Core.Gameplay.TBS;
using Game.Core.Systems.CommandSystem;
using Game.GamePipeline;
using UnityEngine;
using Zenject;

namespace Game.Installers
{
    public sealed class BattleTBSInstaller : MonoInstaller< BattleTBSInstaller >
    {
        [ SerializeField ] private TBSGameplayConfig _config;
        
        public override void InstallBindings()
        {
            Debug.Log( "[GamePipeline] BattleInstaller" );

            Container.BindInstance( _config );
            
            CommandSystemInstaller.Install( Container );
            
            Container.Bind< UnitFactory >().AsSingle().Lazy();
            
            Container.BindInterfacesAndSelfTo< BootstrapBattle >().AsSingle().NonLazy();
        }
    }
}