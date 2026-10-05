namespace Game.Core.Systems.StateSystem
{
    public abstract class State : IState
    {
        public virtual void Enter() { }
        public virtual void Exit() { }

        public virtual void Tick( float dt ) { }
        public virtual void FixedTick( float dt) { }
    }
}