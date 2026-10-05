using Cysharp.Threading.Tasks;

namespace Game.Core.Gameplay.TBS
{
    public abstract class BattleAction
    {
        public UnitController Owner { get; }
        public virtual bool CompletesUnitTurn => false;

        protected BattleAction(UnitController owner)
        {
            Owner = owner;
        }

        public abstract bool CanExecute( BattleActionTarget target );

        public abstract UniTask Execute( BattleActionTarget target );
    }
}
