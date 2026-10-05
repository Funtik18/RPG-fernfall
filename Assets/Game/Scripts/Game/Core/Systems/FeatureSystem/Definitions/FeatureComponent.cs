using System;
using UnityEngine;

namespace Game.Core.Systems.FeatureSystem
{
    public abstract class FeatureComponent : MonoBehaviour
    {
        public abstract Type GetFeatureType();
    }
    
    public abstract class FeatureComponentPresenter< T > : IFeaturePresenter
        where T : FeatureComponent
    {
        public T Component { get; }

        public FeatureComponentPresenter( T component )
        {
            Component = component ?? throw new ArgumentNullException( nameof(component) );
        }
    }
}