namespace MyPriorityQueue;

using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Realisation of thread-safe priority queue interface
/// </summary>
/// <typeparam name="TValue">Type of value</typeparam>
public class PriorityQueue<TValue> : IPriorityQueue<TValue>
{
    private readonly SortedList<int, Queue<TValue>> elementsByPriority = new();
    private int counter = 0;
    private readonly object locker = new();

    public void Enqueue(TValue value, int priority)
    {
        lock (locker)
        {
            if (!elementsByPriority.ContainsKey(priority))
            {
                elementsByPriority[priority] = new Queue<TValue>();
            }
            elementsByPriority[priority].Enqueue(value);
            counter++;
            Monitor.PulseAll(locker);
        }
    }

    public TValue Dequeue()
    {
        lock (locker)
        {
            while (counter == 0)
            {
                Monitor.Wait(locker);
            }

            var maxPriority = elementsByPriority.Keys.Last();
            var element = elementsByPriority[maxPriority].Dequeue();

            if (elementsByPriority[maxPriority].Count == 0)
            {
                elementsByPriority.Remove(maxPriority);
            }
            counter--;

            return element;
        }
    }

    public int Size => counter;
}
