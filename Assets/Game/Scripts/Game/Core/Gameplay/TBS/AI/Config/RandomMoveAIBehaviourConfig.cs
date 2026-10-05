using System;
using UnityEngine;

namespace Game.Core.Gameplay.TBS
{
    [ CreateAssetMenu( fileName = "RandomMoveAIBehaviourConfig", menuName = "Game/TBS/AI/Random Move" ) ]
    public sealed class RandomMoveAIBehaviourConfig : AIBehaviourConfig
    {
        public override Type BehaviourType => typeof(RandomMoveAIBehaviour);
    }
}
