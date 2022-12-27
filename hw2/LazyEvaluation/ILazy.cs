namespace LazyEvaluation;

/// <summary>
/// Interface for lazy evaluation
/// </summary>
public interface ILazy<T>
{
    /// <summary>
    /// Calculates functioon and returns the result
    /// </summary>
    /// <returns>The result of calculation</returns>
    T? Get();
}
