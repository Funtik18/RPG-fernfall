using Zenject;

namespace Game.Core.Systems.SheetSystem
{
    public sealed class SkillFactory
    {
        private readonly DiContainer _diContainer;
        
        public SkillFactory( DiContainer diContainer )
        {
            _diContainer = diContainer ?? throw new System.ArgumentNullException( nameof(diContainer) );
        }

        public Skill Create( SkillConfig config )
        {
            return (Skill)_diContainer.Instantiate( config.GetSkillType(), new object[] { config } );
        }
    }
}