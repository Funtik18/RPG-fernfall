using System;
using UnityEngine;

namespace Game.Core.Gameplay.TBS
{
    public abstract class AIBehaviourConfig : ScriptableObject
    {
        [ SerializeField, TextArea( 3, 8 ) ] private string _description;

        public string Description => _description;

        public abstract Type BehaviourType { get; }
    }
}
