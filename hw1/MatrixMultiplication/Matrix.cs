namespace MatrixMultiplication;

using System;
using System.Threading;

/// <summary>
/// Class of matrix
/// </summary>
public class Matrix
{
    private int[,] matrix;

    /// <summary>
    /// The number of rows in matrix
    /// </summary>
    public int Rows { get { return matrix.GetLength(0); } }

    /// <summary>
    /// The number of columns in matrix
    /// </summary>
    public int Columns { get { return matrix.GetLength(1); } }

    /// <summary>
    /// The empty matrix constructor
    /// </summary>
    /// <param name="rows">The number of rows</param>
    /// <param name="cols">The number of columns</param>
    /// <exception cref="ArgumentException">Exception if matrix sizes are incorrect</exception>
    public Matrix(int rows, int cols)
    {
        if (rows <= 0 || cols <= 0)
        {
            throw new ArgumentException("Matrix size can not be negative!");
        }
        matrix = new int[rows, cols];
    }

    /// <summary>
    /// Construct matrix by path to file
    /// </summary>
    /// <param name="path">Path to file with matrix</param>
    /// <exception cref="ArgumentException">Exception if matrix sizes are incorrect</exception>
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

    /// <summary>
    /// Checking correction of matrix sizes
    /// </summary>
    /// <param name="matrix">Given matrix</param>
    /// <exception cref="ArgumentException">Exception if matrix sizes are incorrect</exception>
    private static void CheckCorrectionOfSizes(Matrix matrix)
    {
        if (matrix.Rows <= 0 || matrix.Columns <= 0)
        {
            throw new ArgumentException("Cannot create matrix with such sizes!");
        }
    }

    /// <summary>
    /// Checking the correction in multiplication
    /// </summary>
    /// <param name="leftMatrix">Left matrix in multiplication</param>
    /// <param name="rightMatrix">Right matrix in multiplication</param>
    /// <exception cref="ArgumentException">Exception if matrix sizes are incorrect</exception>
    private static void CheckMultiplicationSize(Matrix leftMatrix, Matrix rightMatrix)
    {
        CheckCorrectionOfSizes(leftMatrix);
        CheckCorrectionOfSizes(rightMatrix);
        if (leftMatrix.Columns != rightMatrix.Rows)
        {
            throw new ArgumentException("Cannot multiply such matrices!");
        }
    }

    /// <summary>
    /// Sequential multiplication of matrices
    /// </summary>
    /// <param name="leftMatrix">Left matrix in multiplication</param>
    /// <param name="rightMatrix">Right matrix in multiplication</param>
    /// <returns>Multiplicated matrix</returns>
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

    /// <summary>
    /// Parallel multiplication of matrices
    /// </summary>
    /// <param name="leftMatrix">Left matrix in multiplication</param>
    /// <param name="rightMatrix">Right matrix in multiplication</param>
    /// <returns>Multiplicated matrix</returns>
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

    /// <summary>
    /// Checking if matrices are equal
    /// </summary>
    /// <param name="firstMatrix">First matrix</param>
    /// <param name="secondMatrix">Second matrix</param>
    /// <returns>The result of comparison</returns>
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

    /// <summary>
    /// Writes the matrix to file
    /// </summary>
    /// <param name="path">Path to file</param>
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

    /// <summary>
    /// The random matrix constructor
    /// </summary>
    /// <param name="rows">The number of rows</param>
    /// <param name="cols">The number of columns</param>
    /// <returns>The random matrix</returns>
    /// <exception cref="ArgumentException">Exception if matrix sizes are incorrect</exception>
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
