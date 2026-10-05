using Game.Core.Systems.CommandSystem;
using UnityEngine;

namespace Game.Core.Gameplay.World.Entity
{
    public sealed class EntityObject : InteractableTriggerObject
    {
        [ field: Header( "Main" ) ]
        [ field: SerializeField ] public string UID { get; private set; }
        // [ field: Header( "Commands" ) ]
        // [ field: SerializeField ] public Rigidbody Rigidbody { get; private set; }
        [ field: Header( "Commands" ) ]
        [ field: SerializeField ] public CommandComponent OnTriggerEnterCommand { get; private set; }
        [ field: SerializeField ] public CommandComponent OnTriggerExitCommand { get; private set; }
        
        public EntityController Controller { get; private set; }

        public void SetController( EntityController controller )
        {
            Controller = controller;
        }

        public void Initialize()
        {
            Controller.Initialize();
        }
        
        public void Dispose()
        {
            Controller?.Dispose();
        }

        public override void OnTriggerEnter() => Controller.OnTriggerEnter();

        public override void OnTriggerExit() => Controller.OnTriggerExit();
    }
}