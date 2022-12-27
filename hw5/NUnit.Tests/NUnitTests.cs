namespace NUnit.Tests;

using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;

/// <summary>
/// Class with tests to test MyNUnit program
/// </summary>
public class NUnitTests
{
    private List<TestInfo> testsResults = new();
    private List<string> methodsToTest = new();
    private string testPath = Path.GetRelativePath(Directory.GetCurrentDirectory(), "..\\..\\..\\..\\ExampleProject.Tests");
    private MyNUnit nunit = new();
    [SetUp]
    public void Setup()
    {
        testsResults = new();
        methodsToTest = new();
        nunit = new();
        methodsToTest.Add("SuccessfulMethod");
        methodsToTest.Add("IgnoredMethod");
        methodsToTest.Add("IgnoredMethodWithUnexpectingException");
        methodsToTest.Add("MethodWithExpectedException");
        methodsToTest.Add("MethodFaildWithException");
        methodsToTest.Add("MethodWithUnexpectedException");
    }

    [Test]
    public void TestOnlyThoseMethodsThatWeNeed()
    {
        nunit.RunTests(testPath);
        var resultOfTesting = nunit.ResultsOfTesting();
        foreach (var method in resultOfTesting.Values)
        {
            foreach (var element in method)
            {
                testsResults.Add(element);
            }
        }
        var names = new List<string>();
        foreach (var element in testsResults)
        {
            names.Add(element.MethodName);
        }
        Assert.AreEqual(methodsToTest.Count, names.Intersect(methodsToTest).Count());
    }

    [Test]
    public void SuccsessfulMethodsPassed()
    {
        nunit.RunTests(testPath);
        var resultOfTesting = nunit.ResultsOfTesting();
        foreach (var method in resultOfTesting.Values)
        {
            foreach (var element in method)
            {
                testsResults.Add(element);
            }
        }
        var succsessful = testsResults.Find(i => i.MethodName == "SuccessfulMethod");
        Assert.IsTrue(succsessful!.IsSuccessful);
    }

    [Test]
    public void IgnoredMethodsAreIgnored()
    {
        nunit.RunTests(testPath);
        var resultOfTesting = nunit.ResultsOfTesting();
        foreach (var method in resultOfTesting.Values)
        {
            foreach (var element in method)
            {
                testsResults.Add(element);
            }
        }
        var ignored = testsResults.Find(i => i.MethodName == "IgnoredMethod");
        var ignoredWithException = testsResults.Find(i => i.MethodName == "IgnoredMethodWithUnexpectingException");
        Assert.IsTrue(ignored!.IsIgnored);
        Assert.AreEqual("This test must be ignored", ignored.ReasonToIgnore);
        Assert.IsTrue(ignoredWithException!.IsIgnored);
        Assert.AreEqual("This test must be ignored", ignoredWithException.ReasonToIgnore);
    }

    [Test]
    public void MethodsWithExpectedException()
    {
        nunit.RunTests(testPath);
        var resultOfTesting = nunit.ResultsOfTesting();
        foreach (var method in resultOfTesting.Values)
        {
            foreach (var element in method)
            {
                testsResults.Add(element);
            }
        }
        var expectedException = testsResults.Find(i => i.MethodName == "MethodWithExpectedException");
        Assert.AreEqual(expectedException!.ExpectedException, expectedException.ActualException);
        Assert.IsTrue(expectedException.IsSuccessful);
    }

    [Test]
    public void FailedMethods()
    {
        nunit.RunTests(testPath);
        var resultOfTesting = nunit.ResultsOfTesting();
        foreach (var method in resultOfTesting.Values)
        {
            foreach (var element in method)
            {
                testsResults.Add(element);
            }
        }
        var failed = testsResults.Find(i => i.MethodName == "MethodFaildWithException");
        Assert.IsFalse(failed!.IsSuccessful);
        Assert.IsNull(failed.ExpectedException);
        Assert.IsNotNull(failed.ActualException);
    }

    [Test]
    public void MethodsWithUnexpectedException()
    {
        nunit.RunTests(testPath);
        var resultOfTesting = nunit.ResultsOfTesting();
        foreach (var method in resultOfTesting.Values)
        {
            foreach (var element in method)
            {
                testsResults.Add(element);
            }
        }
        var unexpectedException = testsResults.Find(i => i.MethodName == "MethodWithUnexpectedException");
        Assert.AreNotEqual(unexpectedException!.ExpectedException, unexpectedException.ActualException);
        Assert.IsFalse(unexpectedException.IsSuccessful);
    }
}