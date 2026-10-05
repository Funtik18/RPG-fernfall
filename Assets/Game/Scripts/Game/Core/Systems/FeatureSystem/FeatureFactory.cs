using System;
using Zenject;

namespace Game.Core.Systems.FeatureSystem
{
    public sealed class FeatureFactory
    {
        private readonly DiContainer _diContainer;
        
        public FeatureFactory( DiContainer diContainer )
        {
            _diContainer = diContainer ?? throw new System.ArgumentNullException( nameof(diContainer) );
        }
        
        public IFeaturePresenter Create( FeatureComponent component )
        {
            return (IFeaturePresenter)_diContainer.Instantiate( component.GetFeatureType(), new object[] { component } );
        }

        public IFeaturePresenter Create( FeatureComponent component, params object[] extraArgs )
        {
            var args = new object[extraArgs.Length + 1];
            args[ 0 ] = component;

            Array.Copy( extraArgs, 0, args, 1, extraArgs.Length );

            return (IFeaturePresenter)_diContainer.Instantiate( component.GetFeatureType(), args );
        }

        public IFeaturePresenter Create( FeatureConfig config )
        {
            return (IFeaturePresenter)_diContainer.Instantiate( config.GetFeatureType(), new object[] { config } );
        }
        
        public IFeaturePresenter Create( FeatureConfig config, params object[] extraArgs )
        {
            var args = new object[extraArgs.Length + 1];
            args[ 0 ] = config;

            Array.Copy( extraArgs, 0, args, 1, extraArgs.Length );

            return (IFeaturePresenter)_diContainer.Instantiate( config.GetFeatureType(), args );
        }
    }
}