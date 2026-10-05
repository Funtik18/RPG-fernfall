using System;
using UnityEngine;

namespace Game.Core.Gameplay.TBS
{
    [ CreateAssetMenu( fileName = "RangerAIBehaviourConfig", menuName = "Game/TBS/AI/Ranger" ) ]
    public sealed class RangerAIBehaviourConfig : AIBehaviourConfig
    {
        public override Type BehaviourType => typeof(RangerAIBehaviour);
    }
}
