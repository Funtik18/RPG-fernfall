using Zenject;

namespace Game.Managers.CampaignManager
{
    public sealed class CampaignManagerInstaller : Installer< CampaignManagerInstaller >
    {
        public override void InstallBindings()
        {
            Container.Bind< CampaignManager >().AsSingle().Lazy();
        }
    }
}