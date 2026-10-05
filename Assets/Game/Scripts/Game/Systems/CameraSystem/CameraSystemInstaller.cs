using Zenject;

namespace Game.Systems.CameraSystem
{
    public sealed class CameraSystemInstaller : Installer< CameraSystemInstaller >
    {
        public override void InstallBindings()
        {
            Container.Bind< CameraFacingService >().AsSingle().Lazy();
        }
    }
}