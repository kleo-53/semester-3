namespace MatrixMultiplication;

using System;
using System.Diagnostics;

/// <summary>
/// Class to comparing methods
/// </summary>
public static class Comparison
{
    /// <summary>
    /// Compares sequental and parallel multiplying methods 
    /// </summary>
    /// <param name="path">Path to file with results of comparation</param>
    public static void Compare(string path)
    {
        var numbeOfRepeating = 10;
        var time = new Stopwatch();

        using var file = new StreamWriter(path);
        file.WriteLine($"Testing of multiplying methods. Each test is executed {numbeOfRepeating} times.");
        file.WriteLine("| Test number | Size of matrix | Average time (sequental) | Square deviation " +
            "(sequental) | Average time (parallel) | Square deviation (parallel) |");
        for (int i = 100; i < 1001; i += 100)
        {
            var leftMatrix = Matrix.RandomMatrixCreate(i, i);
            var rightMatrix = Matrix.RandomMatrixCreate(i, i);
            var mathExpectationSequental = 0.0;
            var mathExpectationParallel = 0.0;
            var squareDeviationSequental = 0.0;
            var squareDeviationParallel = 0.0;

            for (int j = 0; j < numbeOfRepeating; j++)
            {
                time.Restart();
                var resultMatrix = Matrix.SequentialMultiplication(leftMatrix, rightMatrix);
                time.Stop();
                var currentTime = time.Elapsed.TotalSeconds;
                mathExpectationSequental += currentTime;
                squareDeviationSequental += currentTime * currentTime;

                time.Restart();
                resultMatrix = Matrix.ParallelMultiplication(leftMatrix, rightMatrix);
                time.Stop(); 
                currentTime = time.Elapsed.TotalSeconds;
                mathExpectationParallel += currentTime;
                squareDeviationParallel += currentTime * currentTime;
            }
            mathExpectationSequental /= numbeOfRepeating;
            mathExpectationParallel /= numbeOfRepeating;
            squareDeviationSequental /= numbeOfRepeating;
            squareDeviationParallel /= numbeOfRepeating;
            squareDeviationSequental -= mathExpectationSequental * mathExpectationSequental;
            squareDeviationParallel -= mathExpectationParallel * mathExpectationParallel;
            squareDeviationSequental = Math.Sqrt(squareDeviationSequental);
            squareDeviationParallel = Math.Sqrt(squareDeviationParallel);
            file.Write(i < 9000 ? $"|      {i / 100}      |     {i}x{i}    |         {mathExpectationSequental:f5} sec       |          {squareDeviationSequental:f5} sec         |" +
                $"       {mathExpectationParallel:f5} sec       |         {squareDeviationParallel:f5} sec         |" : $"|     {i / 100}    " +
                $"  |    {i}x{i}   |        {mathExpectationSequental:f5} sec       |          {squareDeviationSequental:f5} sec         |" +
                $"       {mathExpectationParallel:f5} sec       |         {squareDeviationParallel:f5} sec         |");
            file.WriteLine();
        }
    }
}
