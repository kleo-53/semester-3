namespace ThreadPool;

/// <summary>
/// Interface of the MyTask
/// </summary>
/// <typeparam name="TResult"></typeparam>
public interface IMyTask<TResult>
{
    /// <summary>
    /// Executes the MyTask
    /// </summary>
    public void Calculate();

    /// <summary>
    /// Returns the result of the MyTask execution
    /// </summary>
    public TResult? Result { get; }

    /// <summary>
    /// Accepts a function that can be applied to the result of a given function
    /// </summary>
    /// <typeparam name="TNewResult">Type of the result of function</typeparam>
    /// <param name="function">Given function</param>
    /// <returns>New MyTask to execute</returns>
    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> function);
}
