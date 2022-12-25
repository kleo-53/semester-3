using MatrixMultiplication;

try
{
    if (args.Length > 0)
    {
        if (String.Compare(args[0], "Comparison") == 0)
        {
            Comparison.Compare("./ResultOfComparison.txt");
            Console.WriteLine("You can find the result of the comparison in the \"ResultOfComparison.txt\"");
        }
    }

    var leftMatrix = new Matrix("./LeftMatrix.txt");
    var rightMatrix = new Matrix("./RightMatrix.txt");
    var resultMatrix = Matrix.SequentialMultiplication(leftMatrix, rightMatrix);
    resultMatrix = Matrix.ParallelMultiplication(leftMatrix, rightMatrix);
    resultMatrix.WriteMatrix("./ResultMatrix.txt");
}
catch (ArgumentException e)
{
    Console.WriteLine(e.Message);
}