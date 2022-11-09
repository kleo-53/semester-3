namespace MD5;

using System;

/// <summary>
/// Class of MD5 counter
/// </summary>
public class Counter
{
    private string currentPath;

    /// <summary>
    /// Bilds the counter
    /// </summary>
    /// <param name="path">Path to file or directory</param>
    public Counter(string path)
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

            foreach (string fileName in fileEntries)
            {
                result += DFS(fileName, result);
            }

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
