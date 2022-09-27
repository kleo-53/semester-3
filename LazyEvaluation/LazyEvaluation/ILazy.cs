namespace LazyEvaluation;

/// <summary>
/// Interface for lazy evaluation
/// </summary>
public interface ILazy<T>
{
    /// <summary>
    /// Lazy evaluation
    /// </summary>
    /// <returns>Result of the calculation</returns>
    T? Get();
}
