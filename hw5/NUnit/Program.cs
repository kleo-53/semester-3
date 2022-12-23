namespace NUnit;

public class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0 || args.Length > 1)
        {
            throw new InvalidDataException("Program must have one input argument");
        }

        Console.WriteLine("Test Running ...");
        MyNUnit.RunTests(args[0]);
        MyNUnit.PrintResults();
    }
}