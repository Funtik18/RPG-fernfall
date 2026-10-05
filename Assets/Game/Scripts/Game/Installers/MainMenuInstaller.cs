using Game.GamePipeline;
using UnityEngine;
using Zenject;

namespace Game.Installers
{
    public sealed class MainMenuInstaller : MonoInstaller< MainMenuInstaller >
    {
        public override void InstallBindings()
        {
            Debug.Log( "[GamePipeline] MainMenuInstaller" );

            Container.BindInterfacesAndSelfTo< BootstrapMainMenu >().AsSingle().NonLazy();
        }
    }
}