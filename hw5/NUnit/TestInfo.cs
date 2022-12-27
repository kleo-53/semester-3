namespace NUnit;

using System;

/// <summary>
/// This class contains information about testing methods
/// </summary>
public class TestInfo
{
    /// <summary>
    /// Name of the method
    /// </summary>
    public string MethodName { get; private set; }

    /// <summary>
    /// If the method returns the expected exception, it would be in this field
    /// </summary>
    public Type? ExpectedException { get; private set; }

    /// <summary>
    /// Shows if the method returns the unexpected exception
    /// </summary>
    public Type? ActualException { get; private set; }

    /// <summary>
    /// Shows if the method was ignored
    /// </summary>
    public bool IsIgnored { get; private set; }

    /// <summary>
    /// If the method was ignored, explanation would be in this field
    /// </summary>
    public string ReasonToIgnore { get; private set; }

    /// <summary>
    /// Shows if the test was succsessful
    /// </summary>
    public bool IsSuccessful { get; private set; }

    /// <summary>
    /// Amount of the test execution time
    /// </summary>
    public TimeSpan Time { get; private set; }

    /// <summary>
    /// Constructor for ignored test methods
    /// </summary>
    public TestInfo(string name, string ignoranceReason)
    {
        MethodName = name;
        IsIgnored = true;
        ReasonToIgnore = ignoranceReason;
    }

    /// <summary>
    /// Constructor for executed test methods
    /// </summary>
    public TestInfo(string name, bool isSuccessful, Type expectedException, Type actualException, TimeSpan time)
    {
        IsIgnored = false;
        ReasonToIgnore = "";

        MethodName = name;
        IsSuccessful = isSuccessful;
        ExpectedException = expectedException;
        ActualException = actualException;
        Time = time;
    }
}
