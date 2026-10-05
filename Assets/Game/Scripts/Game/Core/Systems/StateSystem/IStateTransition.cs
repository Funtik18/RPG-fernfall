namespace Game.Core.Systems.StateSystem
{
    public interface IStateTransition
    {
        IState From { get; }
        IState To { get; }

        bool CanTransition();
    }
}