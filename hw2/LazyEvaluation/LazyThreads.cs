namespace LazyEvaluation;

using System;

/// <summary>
/// Class which make calculation with a synchronization
/// </summary>
public class LazyThreads<T> : ILazy<T>
{
    /// <summary>
    /// Result of calculation
    /// </summary>
    private T? result;

    /// <summary>
    /// Givan function
    /// </summary>
    private Func<T?> supplier;

    /// <summary>
    /// Checker of calculation
    /// </summary>
    private bool wasCalculated = false;

    /// <summary>
    /// Constructor of class
    /// </summary>
    /// <param name="supplier">Constructor of function</param>
    public LazyThreads(Func<T> supplier) => this.supplier = supplier;

    /// <summary>
    /// Object to lock
    /// </summary>
    private object locker = new object();

    public T? Get()
    {
        if (wasCalculated)
        {
            return result;
        }
        lock (locker)
        {
            if (wasCalculated)
            {
                return result;
            }
            result = supplier();
            wasCalculated = true;
            return result;
        }
    }
}
