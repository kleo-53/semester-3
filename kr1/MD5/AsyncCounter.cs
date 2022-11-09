namespace MD5;

using System;
using System.Threading.Tasks;

/// <summary>
/// Class of parallel MD5 counter
/// </summary>
public class AsyncCounter
{
    private string currentPath;

    /// <summary>
    /// Bilds the counter
    /// </summary>
    /// <param name="path">Path to file or directory</param>
    public AsyncCounter(string path)
    {
        currentPath = path;
    }

    /// <summary>
    /// Calculate the hex of MD5-sum
    /// </summary>
    /// <param name="path">Path to file or directory</param>
    /// <param name="result">Current result</param>
    /// <returns>The result of calculation</returns>
    private static string DFS(string path, string result)
    {
        if (File.Exists(path))
        {
            var md5 = System.Security.Cryptography.MD5.Create();
            var bytes = File.ReadAllBytes(path);
            var hashBytes = md5.ComputeHash(bytes);
            md5.Clear();
            return Convert.ToHexString(hashBytes);
        }
        else if (Directory.Exists(path))
        {
            string[] fileEntries = Directory.GetFiles(path);
            string[] subdirectoryEntries = Directory.GetDirectories(path);
            Parallel.ForEach(fileEntries, fileName =>
            {
                result += DFS(fileName, result);
            });

            foreach (string subdirectory in subdirectoryEntries)
            {
                result += DFS(subdirectory, result);
            }
        }
        return result;
    }

    /// <summary>
    /// Starts the program
    /// </summary>
    /// <returns>Result of calculation</returns>
    public string Start()
    {
        var result = "";
        return DFS(currentPath, result);
    }
}
