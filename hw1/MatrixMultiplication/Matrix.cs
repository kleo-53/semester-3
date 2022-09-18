namespace MatrixMultiplication;

using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Matrix
{
    private int[,] matrix;

    public int Rows { get { return matrix.GetLength(0); } }

    public int Columns { get { return matrix.GetLength(1); } }

    public Matrix(int rows, int cols)
    {
        if (rows <= 0 || cols <= 0)
        {
            throw new ArgumentException("Matrix size can not be negative!");
        }
        matrix = new int[rows, cols];
    }

    public Matrix(string path)
    {
        using StreamReader file = new(path);
        var lineWithSizes = file.ReadLine();
        if (lineWithSizes == null)
        {
            throw new ArgumentException("Matrix file is empty!");
        }

        var matrixSizes = lineWithSizes.Split(' ');
        if (matrixSizes.Length != 2)
        {
            throw new ArgumentException("Invalid number of matrix sizes!");
        }

        var rows = Int32.Parse(matrixSizes[0]);
        var columns = Int32.Parse(matrixSizes[1]);
        if (rows <= 0 || columns <= 0)
        {
            throw new ArgumentException("Invalid value of matrix sizes!");
        }

        matrix = new int[rows, columns];

        for (int i = 0; i < rows; i++)
        {
            var readLine = file.ReadLine();
            if (readLine == null)
            {
                throw new ArgumentException("Matrix is incorrect!");
            }
            var currentRow = readLine.Split(' ');
            if (currentRow.Length != columns)
            {
                throw new ArgumentException("Invalid number of matrix values!");
            }
            for (int j = 0; j < columns; j++)
            {
                matrix[i, j] = Int32.Parse(currentRow[j]);
            }
        }
    }

    private static void CheckCorrectionOfSizes(Matrix matrix)
    {
        if (matrix.Rows <= 0 || matrix.Columns <= 0)
        {
            throw new ArgumentException("Cannot create matrix with such sizes!");
        }
    }

    private static void CheckMultiplicationSize(Matrix leftMatrix, Matrix rightMatrix)
    {
        CheckCorrectionOfSizes(leftMatrix);
        CheckCorrectionOfSizes(rightMatrix);
        if (leftMatrix.Columns != rightMatrix.Rows)
        {
            throw new ArgumentException("Cannot multiply such matrices!");
        }
    }

    public static Matrix SequentialMultiplication(Matrix leftMatrix, Matrix rightMatrix)
    {
        CheckMultiplicationSize(leftMatrix, rightMatrix);
        Matrix resultMartix = new Matrix(leftMatrix.Rows, rightMatrix.Columns);
        for (int row = 0; row < leftMatrix.Rows; row++)
        {
            for (int col = 0; col < rightMatrix.Columns; col++)
            {
                for (int k = 0; k < leftMatrix.Columns; k++)
                {
                    resultMartix.matrix[row, col] += leftMatrix.matrix[row, k] * rightMatrix.matrix[k, col];
                }
            }
        }
        return resultMartix;
    }

    public static Matrix ParallelMultiplication(Matrix leftMatrix, Matrix rightMatrix)
    {
        CheckMultiplicationSize(leftMatrix, rightMatrix);
        Matrix resultMartix = new Matrix(leftMatrix.Rows, rightMatrix.Columns);
        var threads = leftMatrix.Rows < Environment.ProcessorCount ? new Thread[leftMatrix.Rows] : new Thread[Environment.ProcessorCount];
        for (int i = 0; i < threads.Length; ++i)
        {
            var localI = i;
            threads[i] = new Thread(() =>
                {
                    for (int row = localI; row < leftMatrix.Rows; row += threads.Length)
                    {
                        for (int col = 0; col < rightMatrix.Columns; ++col)
                        {
                            for (int k = 0; k < leftMatrix.Columns; ++k)
                            {
                                resultMartix.matrix[row, col] += leftMatrix.matrix[row, k] * rightMatrix.matrix[k, col];
                            }
                        }
                    }
                });
        }
        foreach (var thread in threads)
        {
            thread.Start();
        }
        foreach (var thread in threads)
        {
            thread.Join();
        }
        return resultMartix;
    }

    public static bool AreEqual(Matrix firstMatrix, Matrix secondMatrix)
    {
        if (firstMatrix.Rows != secondMatrix.Rows || firstMatrix.Columns != secondMatrix.Columns)
        {
            return false;
        }
        for (int i = 0; i < secondMatrix.Rows; ++i)
        {
            for (int j = 0; j < secondMatrix.Columns; ++j)
            {
                if (secondMatrix.matrix[i, j] != firstMatrix.matrix[i, j])
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void WriteMatrix(string path)
    {
        using StreamWriter writer = new(path);
        writer.WriteLine($"{Rows} {Columns}");
        for (int i = 0; i < Rows; ++i)
        {
            for (int j = 0; j < Columns; ++j)
            {
                writer.Write($"{matrix[i, j]} ");
            }
            writer.WriteLine();
        }
    }

    private static Random random = new();

    public static Matrix RandomMatrixCreate(int rows, int cols)
    {
        if (rows <= 0 || cols <= 0)
        {
            throw new ArgumentException("Cannot create matrix with such sizes!");
        }
        var matrix = new Matrix(rows, cols);
        for (var i = 0; i < rows; ++i)
        {
            for (var j = 0; j < cols; ++j)
            {
                matrix.matrix[i, j] = random.Next() % 10000;
            }
        }
        return matrix;
    }
}
