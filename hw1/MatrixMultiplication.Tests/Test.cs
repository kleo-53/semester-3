namespace MatrixMultiplication.Tests;

using NUnit.Framework;
using System;

public class Tests
{
    [TestCase(-3, 50)]
    [TestCase(3, -5)]
    [TestCase(0, 0)]
    public void InvalidArgumentsMakeArgumetsExceptionsInCreatingMatrix(int rowLeft, int colLeft)
    {
        Assert.Throws<ArgumentException>(() => new Matrix(rowLeft, colLeft));
    }

    [TestCase(50, 50, 5, 8)]
    [TestCase(3, 5, 3, 5)]
    [TestCase(80, 2, 3, 80)]
    [TestCase(50, 2, 1, 80)]
    public void InvalidArgumentsMakeArgumetsExceptionsInMultiplying(int rowLeft, int colLeft, int rowRight, int colRight)
    {
        var leftMatrix = new Matrix(rowLeft, colLeft);
        var rightMatrix = new Matrix(rowRight, colRight);
        Assert.Throws<ArgumentException>(() => Matrix.SequentialMultiplication(leftMatrix, rightMatrix));
        Assert.Throws<ArgumentException>(() => Matrix.ParallelMultiplication(leftMatrix, rightMatrix));
    }

    [TestCase(50, 50, 50, 50)]
    [TestCase(2, 160, 160, 2)]
    [TestCase(576, 12, 12, 430)]
    [TestCase(1, 1, 1, 1)]
    public void MultipliedMatricesAreEqual(int rowLeft, int colLeft, int rowRight, int colRight)
    {
        var leftMatrix = new Matrix(rowLeft, colLeft);
        var rightMatrix = new Matrix(rowRight, colRight);
        var sequentalResult = Matrix.SequentialMultiplication(leftMatrix, rightMatrix);
        var parallelResult = Matrix.ParallelMultiplication(leftMatrix, rightMatrix);
        Assert.IsTrue(Matrix.AreEqual(parallelResult, sequentalResult));
    }
}
