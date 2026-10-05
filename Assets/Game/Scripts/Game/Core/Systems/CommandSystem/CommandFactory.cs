using System;
using System.Collections.Generic;
using System.Linq;
using Zenject;

namespace Game.Core.Systems.CommandSystem
{
    public sealed class CommandFactory
    {
        private readonly DiContainer _diContainer;
        
        public CommandFactory( DiContainer diContainer )
        {
            _diContainer = diContainer ?? throw new ArgumentNullException( nameof(diContainer) );
        }

        public ICommand Create( CommandComponent component, params object[] extraArgs )
        {
            var args = extraArgs?.Prepend( component ).ToArray() ?? new object[] { component };
            return (ICommand)_diContainer.Instantiate( component.GetPresenterType(), args );
        }
    }
}