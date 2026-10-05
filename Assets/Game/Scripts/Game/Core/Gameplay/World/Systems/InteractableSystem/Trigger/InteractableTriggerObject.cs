namespace Game.Core.Gameplay.World
{
    public abstract class InteractableTriggerObject : InteractableObject
    {
        public virtual void OnTriggerEnter() { }
        public virtual void OnTriggerExit() { }
    }
}