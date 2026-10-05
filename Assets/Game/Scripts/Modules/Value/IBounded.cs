namespace Value
{
    public interface IBounded< T >
    {
        T MinValue { get; }
        T MaxValue { get; }
    }
}