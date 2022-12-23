namespace NUnit;

using NUnit.Attributes;
using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading.Tasks;

/// <summary>
/// Set of class methods required for testing
/// </summary>
public class ClassMethods
{
    /// <summary>
    /// Queue of methods with "BeforeClassAttributes"
    /// </summary>
    public ConcurrentQueue<MethodInfo> BeforeClassTestMethods { get; private set; } = new();

    /// <summary>
    /// Queue of methods with "BeforeAttributes"
    /// </summary>
    public ConcurrentQueue<MethodInfo> BeforeTestMethods { get; private set; } = new();

    /// <summary>
    /// Queue of mwthods with "TestAttributes"
    /// </summary>
    public ConcurrentQueue<MethodInfo> TestMethods { get; private set; } = new();

    /// <summary>
    /// Queue of methods with "AfterAttributes"
    /// </summary>
    public ConcurrentQueue<MethodInfo> AfterTestMethods { get; private set; } = new();

    /// <summary>
    /// Queue of methods with "AfterClassAttributes"
    /// </summary>
    public ConcurrentQueue<MethodInfo> AfterClassTestMethods { get; private set; } = new();

    /// <summary>
    /// Creates the ClassMethods from type object by filling the queues with correct methods attributes
    /// </summary>
    public ClassMethods(Type type)
    {
        Parallel.ForEach(type.GetMethods(), method =>
        {
            if (method.GetCustomAttribute<TestAttribute>() != null)
            {
                TryToEnqueue(method, TestMethods);
            }
            else if (method.GetCustomAttribute<BeforeClassAttribute>() != null)
            {
                if (!method.IsStatic)
                {
                    throw new FormatException("Methods invoked before testing the class must be static.");
                }
                TryToEnqueue(method, BeforeClassTestMethods);
            }
            else if (method.GetCustomAttribute<BeforeAttribute>() != null)
            {
                TryToEnqueue(method, BeforeTestMethods);
            }
            else if (method.GetCustomAttribute<AfterAttribute>() != null)
            {
                TryToEnqueue(method, AfterTestMethods);
            }
            else if (method.GetCustomAttribute<AfterClassAttribute>() != null)
            {
                if (!method.IsStatic)
                {
                    throw new FormatException("Methods invoked after testing the class must be static.");
                }
                TryToEnqueue(method, AfterClassTestMethods);
            }
        });
    }

    /// <summary>
    /// Checks if method is suitable for testing and enqueue if yes
    /// </summary>
    /// <param name="method">Given method</param>
    /// <param name="queue">Given queue</param>
    /// <exception cref="FormatException">Exception if method is not suitable for testing</exception>
    private void TryToEnqueue(MethodInfo method, ConcurrentQueue<MethodInfo> queue)
    {
        if (method.GetParameters().Length != 0 || method.ReturnType != typeof(void))
        {
            throw new FormatException("Method shouldn't return value or get parameters");
        }
        queue.Enqueue(method);
    }
}
