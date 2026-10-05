using Zenject;

namespace Game.Managers.PartyManager
{
    public sealed class PartyManagerInstaller : Installer< PartyManagerInstaller >
    {
        public override void InstallBindings()
        {
            Container.Bind< PartyManager >().AsSingle().Lazy();
        }
    }
}