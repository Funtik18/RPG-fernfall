using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Gameplay.TBS
{
    public sealed class BattleObject : MonoBehaviour
    {
        [ field: SerializeField ] public GridLinks Links { get; private set; }
        [ field: SerializeField ] public List< GridObject > Grids { get; private set; } = new();
        
        public BattleController Controller { get; private set; }
        
        public void SetController( BattleController controller )
        {
            Controller = controller;
        }
    }
}