namespace Game.Core.Systems.StateSystem
{
    public interface IState
    {
        /// <summary>
        /// Triggered when we enter the state.
        /// </summary>
        void Enter();
        
        /// <summary>
        /// Triggered when we exit the state.
        /// </summary>
        void Exit();

        /// <summary>
        /// Update this state and its children with a specified delta time.
        /// </summary>
        void Tick( float dt );
        
        /// <summary>
        /// Update this state and its children with a specified fixed delta time.
        /// </summary>
        void FixedTick( float dt );
    }
}