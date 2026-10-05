namespace Game.Systems.StorageSystem
{
    public interface IMemento
    {
        void Commit();
        void RestoreCommit();
    }
}