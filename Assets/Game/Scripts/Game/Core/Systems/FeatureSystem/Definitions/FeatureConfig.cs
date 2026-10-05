using System;
using UnityEngine;

namespace Game.Core.Systems.FeatureSystem
{
    public abstract class FeatureConfig : ScriptableObject
    {
        public abstract Type GetFeatureType();
    }
    
    public abstract class FeatureConfigPresenter< T > : IFeaturePresenter
        where T : FeatureConfig
    {
        public T Config { get; }

        public FeatureConfigPresenter( T config )
        {
            Config = config ?? throw new ArgumentNullException( nameof(config) );
        }
    }
}