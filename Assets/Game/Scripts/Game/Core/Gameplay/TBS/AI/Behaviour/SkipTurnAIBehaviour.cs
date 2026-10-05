using Cysharp.Threading.Tasks;

namespace Game.Core.Gameplay.TBS
{
    public sealed class SkipTurnAIBehaviour : IAIBehaviour
    {
        public UniTask Execute( AIContext context )
        {
            context.CompleteUnitTurn();
            return UniTask.CompletedTask;
        }
    }
}
