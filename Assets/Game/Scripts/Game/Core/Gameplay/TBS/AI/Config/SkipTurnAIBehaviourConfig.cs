using System;
using UnityEngine;

namespace Game.Core.Gameplay.TBS
{
    [ CreateAssetMenu( fileName = "SkipTurnAIBehaviourConfig", menuName = "Game/TBS/AI/Skip Turn" ) ]
    public sealed class SkipTurnAIBehaviourConfig : AIBehaviourConfig
    {
        public override Type BehaviourType => typeof(SkipTurnAIBehaviour);
    }
}
