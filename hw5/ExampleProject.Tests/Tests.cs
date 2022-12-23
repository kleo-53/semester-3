namespace ExampleProject.Tests;

using TestAttribute = NUnit.Attributes.TestAttribute;
using System;

/// <summary>
/// Class with tests to example project
/// </summary>
public class Tests
{
    [Test]
    public void SuccessfulMethod()
    {
    }

    [Test("This test must be ignored")]
    public void IgnoredMethod()
    {
    }

    [Test("This test must be ignored")]
    public void IgnoredMethodWithUnexpectingException()
    {
        throw new Exception();
    }

    [Test("", typeof(ArgumentNullException))]
    public void MethodWithExpectedException()
    {
        throw new ArgumentNullException();
    }

    [Test]
    public void MethodFaildWithException()
    {
        throw new Exception();
    }

    [Test("", typeof(ArgumentNullException))]
    public void MethodWithUnexpectedException()
    {
        throw new Exception();
    }
}