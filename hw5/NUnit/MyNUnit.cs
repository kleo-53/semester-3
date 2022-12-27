namespace NUnit;

using NUnit.Attributes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

/// <summary>
/// Class of testing methods
/// </summary>
public class MyNUnit
{
    private ConcurrentDictionary<Type, ClassMethods> methodsToTest = new();
    private ConcurrentDictionary<Type, ConcurrentBag<TestInfo>> testResults = new();

    /// <summary>
    /// Gets all assemblies and loads classes
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private IEnumerable<Type> GetAllClasses(string path)
    {
        var assemblies = Directory.EnumerateFiles(path, "*.dll", SearchOption.AllDirectories).ToList();
        assemblies.RemoveAll(assemblyPath => assemblyPath.Contains("\\NUnitProject.dll"));
        assemblies.RemoveAll(assemblyPath => assemblyPath.Contains("\\NUnitProject.Tests.dll"));

        var assembliesNames = new List<string>();
        var differentAssemblies = new List<string>();
        foreach (var assembly in assemblies)
        {
            if (!assembliesNames.Contains(Path.GetFileName(assembly)))
            {
                assembliesNames.Add(Path.GetFileName(assembly));
                differentAssemblies.Add(assembly);
            }
        }
        return differentAssemblies.AsParallel().Select(Assembly.LoadFrom).SelectMany(assembly => assembly.ExportedTypes).Where(type => type.IsClass);
    }

    /// <summary>
    /// Runs the tests
    /// </summary>
    /// <param name="path">Path to assemblies</param>
    public void RunTests(string path)
    {
        var classes = GetAllClasses(path);
        Parallel.ForEach(classes, type =>
        {
            methodsToTest.TryAdd(type, new ClassMethods(type));
        });

        Parallel.ForEach(methodsToTest.Keys, type =>
        {
            testResults.TryAdd(type, new ConcurrentBag<TestInfo>());
            foreach (var beforeClassMethod in methodsToTest[type].BeforeClassTestMethods)
            {
                beforeClassMethod.Invoke(null, null);
            }
            Parallel.ForEach(methodsToTest[type].TestMethods, testMethod =>
            {
                ExecuteTestMethod(type, testMethod);
            });
            foreach (var afterClassMethod in methodsToTest[type].AfterClassTestMethods)
            {
                afterClassMethod.Invoke(null, null);
            }
        });
    }

    /// <summary>
    /// Runs all tests from their classes
    /// </summary>
    private void ExecuteTestMethod(Type type, MethodInfo method)
    {
        var attribute = method.GetCustomAttribute<TestAttribute>();
        var isSuccessful = false;
        Type? thrownException = null;
        var emptyConstructor = type.GetConstructor(Type.EmptyTypes);

        if (emptyConstructor == null)
        {
            throw new FormatException($"{type.Name} must have parameterless constructor");
        }
        if (attribute!.IsIgnored)
        {
            testResults[type].Add(new TestInfo(method.Name, attribute.IgnoreMessage));
            return;
        }

        var instance = emptyConstructor.Invoke(null);
        foreach (var beforeTestMethod in methodsToTest[type].BeforeTestMethods)
        {
            beforeTestMethod.Invoke(instance, null);
        }

        var stopwatch = new Stopwatch();
        stopwatch.Start();
        try
        {
            method.Invoke(instance, null);

            if (attribute.ExpectedException == null)
            {
                isSuccessful = true;
            }
        }
        catch (Exception ex)
        {
            thrownException = ex.InnerException!.GetType();

            if (thrownException == attribute.ExpectedException)
            {
                isSuccessful = true;
            }
        }
        finally
        {
            stopwatch.Stop();
            var ellapsedTime = stopwatch.Elapsed;
            testResults[type].Add(new TestInfo(method.Name, isSuccessful, attribute.ExpectedException!, thrownException!, ellapsedTime));
        }

        foreach (var afterTestMethod in methodsToTest[type].AfterTestMethods)
        {
            afterTestMethod.Invoke(instance, null);
        }
    }

    /// <summary>
    /// Writes the results of testing into dictionary
    /// </summary>
    /// <returns>Results of testing</returns>
    public Dictionary<Type, List<TestInfo>> ResultsOfTesting()
    {
        var result = new Dictionary<Type, List<TestInfo>>();

        foreach (var type in testResults.Keys)
        {
            result.Add(type, new List<TestInfo>());
            foreach (var testInfo in testResults[type])
            {
                result[type].Add(testInfo);
            }
        }
        return result;
    }

    /// <summary>
    /// Prints the results of testing into console
    /// </summary>
    public void PrintResults()
    {
        Console.WriteLine("----Results of testing----");
        foreach (var type in testResults.Keys)
        {
            Console.WriteLine($"Class: {type}");
            foreach (var testInfo in testResults[type])
            {
                Console.WriteLine();
                Console.WriteLine($"Tested method: {testInfo.MethodName}()");
                if (testInfo.IsIgnored)
                {
                    Console.WriteLine($"Ignored test with message: {testInfo.ReasonToIgnore}");
                }
                else
                {
                    Console.WriteLine($"Execution time in milliseconds: { testInfo.Time.TotalMilliseconds}");
                    if (testInfo.ExpectedException != null && testInfo.ActualException != null)
                    {
                        Console.WriteLine($"Expected exception: {testInfo.ExpectedException}\nThrown exception: {testInfo.ActualException}");
                    }
                    if (testInfo.ExpectedException == null && testInfo.ActualException != null)
                    {
                        Console.WriteLine($"Unexpected exception: {testInfo.ActualException}");
                    }
                    if (testInfo.IsSuccessful)
                    {
                        Console.WriteLine($"Test has passed");
                    }
                    else
                    {
                        Console.WriteLine($"Test has failed");
                    }
                }
                
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }
}
