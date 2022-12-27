namespace NUnit.Attributes;

using System;

/// <summary>
/// Attributes to the test methods
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class TestAttribute : Attribute
{
    /// <summary>
    /// Type of the expected exception
    /// </summary>
    public Type? ExpectedException { get; private set; }

    /// <summary>
    /// If the test was ignored this field would contains the message of reason why
    /// </summary>
    public string IgnoreMessage { get; private set; }

    /// <summary>
    /// Information about if the test should be ignored
    /// </summary>
    public bool IsIgnored
        => IgnoreMessage != "";

    /// <summary>
    /// Applies an attribute with the input parameters
    /// </summary>
    /// <param name="message">Shows if test should be ignored.</param>
    /// <param name="expected">Type of the expected exception</param>
    public TestAttribute(string message = "", Type? expected = null)
    {
        ExpectedException = expected;
        IgnoreMessage = message;
    }
}
