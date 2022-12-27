namespace LazyEvaluation;

using System;

/// <summary>
/// Class which make calculation without a synchronization
/// </summary>
public class Lazy<T> : ILazy<T>
{
    /// <summary>
    /// Result of calculation
    /// </summary>
    private T? result;

    /// <summary>
    /// Givan function
    /// </summary>
    private Func<T?>? supplier;

    /// <summary>
    /// Checker of calculation
    /// </summary>
    private bool wasCalculated = false;

    /// <summary>
    /// Constructor of class
    /// </summary>
    /// <param name="supplier">Constructor of function</param>
    public Lazy(Func<T>? supplier) => this.supplier = supplier;

    public T? Get()
    {
        if (wasCalculated)
        {
            return result;
        }
        result = supplier == null ? default(T) : supplier();
        supplier = null;
        wasCalculated = true;
        return result;
    }
}
