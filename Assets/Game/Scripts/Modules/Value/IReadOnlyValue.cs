namespace Value
{
    public interface IReadOnlyValue< T > : IObservableValue
    {
        T Value { get; }
    }
}