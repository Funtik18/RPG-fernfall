using Game.UI.Services;
using UnityEngine;
using Zenject;

namespace Game.UI
{
    [ CreateAssetMenu( fileName = "UIInstaller", menuName = "Installers/UIInstaller" ) ]
    public sealed class UIInstaller : ScriptableObjectInstaller< UIInstaller >
    {
        [ SerializeField ] private UISettings _settings;

        public override void InstallBindings()
        {
            Container.BindInstance( _settings );

            Container.Bind< FadeScreenService >().AsSingle().Lazy();
            Container.Bind< UIFeedbackService >().AsSingle().Lazy();
            Container.Bind< UIManager >().AsSingle().Lazy();
        }
    }
}