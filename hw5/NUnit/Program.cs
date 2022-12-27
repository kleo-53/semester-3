using NUnit;

if (args.Length == 0 || args.Length > 1)
{
    throw new InvalidDataException("Program must have one input argument");
}

Console.WriteLine("Test Running ...");
var nunit = new MyNUnit();
nunit.RunTests(args[0]);
nunit.PrintResults();