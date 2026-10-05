using System;
using UnityEngine;

namespace Game.Core.Gameplay.TBS
{
    [ CreateAssetMenu( fileName = "ChasePlayerAIBehaviourConfig", menuName = "Game/TBS/AI/Chase Player" ) ]
    public sealed class ChasePlayerAIBehaviourConfig : AIBehaviourConfig
    {
        public override Type BehaviourType => typeof(ChasePlayerAIBehaviour);
    }
}
