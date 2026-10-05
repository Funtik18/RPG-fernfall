using Cysharp.Threading.Tasks;
using System.Linq;
using UnityEngine;

namespace Game.Core.Gameplay.TBS
{
    public sealed class RandomMoveAIBehaviour : IAIBehaviour
    {
        public async UniTask Execute( AIContext context )
        {
            var cells = context.GetReachableMoveCells()
                .OrderBy( _ => Random.value )
                .ToArray();

            foreach ( var cell in cells )
            {
                if ( await context.TryMove( cell ) )
                {
                    break;
                }
            }

            context.CompleteUnitTurn();
        }
    }
}
