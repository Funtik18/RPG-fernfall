using Cysharp.Threading.Tasks;

namespace Game.Core.Gameplay.TBS
{
    public interface IAIBehaviour
    {
        UniTask Execute( AIContext context );
    }
}
