using System;
using UnityEngine;

namespace Game.Core.Systems.CommandSystem
{
    public abstract class CommandComponent : MonoBehaviour
    {
        public abstract Type GetPresenterType();
    }
}