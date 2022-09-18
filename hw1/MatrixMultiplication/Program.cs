using MatrixMultiplication;
using System;
using System.Threading;

try
{
    //Comparison.Compare("../../../Comparison.txt");

    var leftMatrix = new Matrix("../../../LeftMatrix.txt");
    var rightMatrix = new Matrix("../../../RightMatrix.txt");
    var resultMatrix = Matrix.SequentialMultiplication(leftMatrix, rightMatrix);
    resultMatrix = Matrix.ParallelMultiplication(leftMatrix, rightMatrix);
    resultMatrix.WriteMatrix("../../../ResultMatrix.txt");
}
catch (ArgumentException e)
{
    Console.WriteLine(e.Message);
}