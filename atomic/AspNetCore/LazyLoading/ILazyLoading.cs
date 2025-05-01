namespace Microsoft.Extensions.DependencyInjection;

public interface ILazyLoading<T> where T : notnull
{
    T Value { get; }
}
