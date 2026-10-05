using Zenject;

namespace Game.Managers.WorldManager
{
    public sealed class WorldManagerInstaller : Installer< WorldManagerInstaller >
    {
        public override void InstallBindings()
        {
            Container.Bind< WorldManager >().AsSingle().Lazy();
        }
    }
}