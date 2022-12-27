namespace MyPriorityQueue;

/// <summary>
/// Interface of thread-safe priority queue
/// </summary>
/// <typeparam name="TValue">Type of value</typeparam>
public interface IPriorityQueue<TValue>
{
    /// <summary>
    /// Add value ant its priority to the priority queue
    /// </summary>
    /// <param name="value">Given value</param>
    /// <param name="priority">Given priority</param>
    void Enqueue(TValue value, int priority);

    /// <summary>
    /// Removes and returns the first founded value with the highest priority from the priority queue
    /// </summary>
    /// <returns>Value with the highest priority</returns>
    TValue Dequeue();

    /// <summary>
    /// Returns the quantity of elements
    /// </summary>
    int Size { get; }
}
